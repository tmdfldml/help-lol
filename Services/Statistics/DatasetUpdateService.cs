using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Logging;

namespace LoLMatchup.Services.Statistics;

public sealed class DatasetUpdateService : IDisposable
{
    private const int ManifestSchemaVersion = 1;
    private const int MaximumManifestBytes = 64 * 1024;
    private const long MaximumDatasetBytes = 128L * 1024 * 1024;
    private readonly DatasetUpdateOptions _options;
    private readonly HttpClient _httpClient;
    private readonly string _datasetPath;
    private readonly string _manifestPath;
    private readonly bool _ownsHttpClient;
    private readonly IAppLogger _logger;

    public DatasetUpdateService(
        DatasetUpdateOptions options,
        string datasetPath,
        HttpClient? httpClient = null, IAppLogger? logger = null)
    {
        _logger = logger ?? NullAppLogger.Instance;
        _options = options;
        _datasetPath = datasetPath;
        _manifestPath = Path.Combine(Path.GetDirectoryName(datasetPath)!, "manifest.json");
        _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        _ownsHttpClient = httpClient is null;
    }

    public async Task<DatasetUpdateCheckResult> CheckAsync(
        long currentRevision,
        CancellationToken cancellationToken = default)
        => await CheckCoreAsync(currentRevision, null, cancellationToken);

    public async Task<DatasetUpdateCheckResult> CheckAsync(
        DatasetUpdateManifest? currentManifest,
        CancellationToken cancellationToken = default)
        => await CheckCoreAsync(
            currentManifest?.DatasetRevision ?? 0,
            currentManifest,
            cancellationToken);

    private async Task<DatasetUpdateCheckResult> CheckCoreAsync(
        long currentRevision, DatasetUpdateManifest? currentManifest, CancellationToken cancellationToken)
    {
        _logger.Log(AppLogLevel.Info, AppLogEvent.ManifestCheckStarted);
        _logger.Log(AppLogLevel.Info, AppLogEvent.CurrentRevision, currentRevision);
        var result = await CheckUnloggedAsync(currentRevision, currentManifest, cancellationToken);
        LogResult(result, cancellationToken);
        return result;
    }

    private async Task<DatasetUpdateCheckResult> CheckUnloggedAsync(
        long currentRevision,
        DatasetUpdateManifest? currentManifest,
        CancellationToken cancellationToken)
    {
        if (_options.ManifestUri is null)
        {
            return new(DatasetUpdateStatus.NotConfigured,
                "업데이트 서버가 아직 설정되지 않았습니다.");
        }

        if (!IsAllowedRemoteUri(_options.ManifestUri))
        {
            return new(DatasetUpdateStatus.InvalidManifest,
                "업데이트 서버 주소가 안전하지 않습니다.");
        }

        try
        {
            using var response = await _httpClient.GetAsync(
                _options.ManifestUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new(DatasetUpdateStatus.NetworkFailure,
                    $"업데이트 서버에 연결하지 못했습니다. (HTTP {(int)response.StatusCode})");
            }

            if (response.Content.Headers.ContentLength is > MaximumManifestBytes)
            {
                return new(DatasetUpdateStatus.InvalidManifest,
                    "업데이트 정보의 크기가 허용 범위를 벗어났습니다.");
            }

            var bytes = await ReadLimitedAsync(
                await response.Content.ReadAsStreamAsync(cancellationToken),
                MaximumManifestBytes,
                cancellationToken);
            var manifest = JsonSerializer.Deserialize<DatasetUpdateManifest>(bytes);
            if (!ValidateManifest(manifest, requireDownloadUrl: true)
                || !TryResolveDownloadUri(manifest!, out _))
            {
                return new(DatasetUpdateStatus.InvalidManifest,
                    "업데이트 정보가 올바르지 않습니다.");
            }

            if (manifest!.DatasetRevision < currentRevision)
            {
                return new(DatasetUpdateStatus.NoDowngrade,
                    "현재 데이터가 서버 데이터보다 최신이므로 변경하지 않습니다.",
                    manifest);
            }

            if (manifest.DatasetRevision == currentRevision)
            {
                if (currentManifest is not null
                    && !HasSameImmutableIdentity(currentManifest, manifest))
                {
                    Debug.WriteLine(
                        "Dataset update immutable revision conflict: the remote manifest "
                        + "does not match the current manifest.");
                    return new(DatasetUpdateStatus.InvalidManifest,
                        "같은 버전의 업데이트 정보가 현재 데이터와 일치하지 않습니다.");
                }

                return new(DatasetUpdateStatus.UpToDate, "최신 데이터입니다.", manifest);
            }

            return new(DatasetUpdateStatus.UpdateAvailable,
                $"새 통계 데이터가 있습니다. ({manifest.AcceptedMatches:N0}경기)",
                manifest);
        }
        catch (JsonException exception)
        {
            Debug.WriteLine($"Dataset manifest JSON failure: {exception.GetType().Name}");
            return new(DatasetUpdateStatus.InvalidManifest,
                "업데이트 정보가 올바르지 않습니다.");
        }
        catch (InvalidDataException exception)
        {
            Debug.WriteLine($"Dataset manifest size guard failure: {exception.GetType().Name}");
            return new(DatasetUpdateStatus.InvalidManifest,
                "업데이트 정보의 크기가 허용 범위를 벗어났습니다.");
        }
        catch (Exception exception) when (exception is HttpRequestException
            or OperationCanceledException or IOException or ObjectDisposedException)
        {
            Debug.WriteLine($"Dataset update check failure: {exception.GetType().Name}");
            return new(DatasetUpdateStatus.NetworkFailure,
                "업데이트 서버에 연결하지 못했습니다. 현재 데이터는 계속 사용할 수 있습니다.");
        }
    }

    public async Task<DatasetUpdateCheckResult> InstallAsync(
        DatasetUpdateManifest manifest, CancellationToken cancellationToken = default)
    {
        _logger.Log(AppLogLevel.Info, AppLogEvent.DownloadStarted, manifest.DatasetRevision);
        var result = await InstallCoreAsync(manifest, cancellationToken);
        LogResult(result, cancellationToken);
        return result;
    }

    private void LogResult(DatasetUpdateCheckResult result, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            _logger.Log(AppLogLevel.Info, AppLogEvent.UpdateCancelled);
            return;
        }
        if (result.Manifest is { } manifest)
            _logger.Log(AppLogLevel.Info, AppLogEvent.ManifestRevision, manifest.DatasetRevision);
        var eventId = result.Status switch
        {
            DatasetUpdateStatus.UpToDate => AppLogEvent.UpToDate,
            DatasetUpdateStatus.NoDowngrade => AppLogEvent.NoDowngrade,
            DatasetUpdateStatus.UpdateAvailable => AppLogEvent.UpdateAvailable,
            DatasetUpdateStatus.Installed => AppLogEvent.InstallSucceeded,
            DatasetUpdateStatus.InvalidManifest or DatasetUpdateStatus.InvalidDataset
                or DatasetUpdateStatus.HashMismatch or DatasetUpdateStatus.SizeMismatch => AppLogEvent.ValidationFailed,
            _ => AppLogEvent.UpdateFailed
        };
        _logger.Log(eventId is AppLogEvent.ValidationFailed or AppLogEvent.UpdateFailed
            ? AppLogLevel.Warning : AppLogLevel.Info, eventId);
        if (result.Status == DatasetUpdateStatus.Installed)
            _logger.Log(AppLogLevel.Info, AppLogEvent.RestartRequired);
    }

    private async Task<DatasetUpdateCheckResult> InstallCoreAsync(
        DatasetUpdateManifest manifest,
        CancellationToken cancellationToken = default)
    {
        if (!ValidateManifest(manifest, requireDownloadUrl: true)
            || !TryResolveDownloadUri(manifest, out var downloadUri))
        {
            return new(DatasetUpdateStatus.InvalidManifest,
                "업데이트 정보가 올바르지 않습니다.");
        }

        var directory = Path.GetDirectoryName(_datasetPath)!;
        var operationId = $"{Environment.ProcessId}-{Guid.NewGuid():N}";
        var temporaryDatasetPath = Path.Combine(directory, $"dataset-{operationId}.download");
        var temporaryManifestPath = Path.Combine(directory, $"manifest-{operationId}.tmp");
        var datasetBackupPath = Path.Combine(directory, $"dataset-{operationId}.rollback");
        var manifestBackupPath = Path.Combine(directory, $"manifest-{operationId}.rollback");
        var datasetExisted = File.Exists(_datasetPath);
        var manifestExisted = File.Exists(_manifestPath);
        var datasetReplaced = false;
        var manifestReplaced = false;

        try
        {
            Directory.CreateDirectory(directory);
            using var response = await _httpClient.GetAsync(
                downloadUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new(DatasetUpdateStatus.NetworkFailure,
                    $"통계 데이터를 다운로드하지 못했습니다. (HTTP {(int)response.StatusCode})");
            }

            if (response.Content.Headers.ContentLength is long declaredLength
                && declaredLength != manifest.FileSizeBytes)
            {
                return new(DatasetUpdateStatus.SizeMismatch,
                    "다운로드한 데이터의 크기가 업데이트 정보와 다릅니다.");
            }

            await using (var source = await response.Content.ReadAsStreamAsync(cancellationToken))
            await using (var destination = new FileStream(
                temporaryDatasetPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true))
            {
                await CopyLimitedAsync(
                    source,
                    destination,
                    manifest.FileSizeBytes,
                    cancellationToken);
                await destination.FlushAsync(cancellationToken);
            }

            var fileInfo = new FileInfo(temporaryDatasetPath);
            if (fileInfo.Length != manifest.FileSizeBytes)
            {
                return new(DatasetUpdateStatus.SizeMismatch,
                    "다운로드한 데이터의 크기가 업데이트 정보와 다릅니다.");
            }

            var hash = await ComputeSha256Async(temporaryDatasetPath, cancellationToken);
            if (!string.Equals(hash, manifest.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                return new(DatasetUpdateStatus.HashMismatch,
                    "다운로드한 데이터의 무결성 검증에 실패했습니다.");
            }

            await using (var stream = File.OpenRead(temporaryDatasetPath))
            {
                var dataset = await JsonSerializer.DeserializeAsync<StatisticsDataset>(
                    stream,
                    cancellationToken: cancellationToken);
                if (!StatisticsDatasetValidator.TryValidate(dataset, out var index, out _)
                    || !MatchesManifest(dataset!, index.Count, manifest))
                {
                    return new(DatasetUpdateStatus.InvalidDataset,
                        "다운로드한 통계 데이터가 올바르지 않아 적용하지 않았습니다.");
                }
            }

            await WriteCompleteFileAsync(
                temporaryManifestPath,
                JsonSerializer.SerializeToUtf8Bytes(manifest),
                cancellationToken);

            ReplaceWithRollback(
                temporaryDatasetPath,
                _datasetPath,
                datasetBackupPath,
                datasetExisted);
            datasetReplaced = true;
            ReplaceWithRollback(
                temporaryManifestPath,
                _manifestPath,
                manifestBackupPath,
                manifestExisted);
            manifestReplaced = true;

            return new(DatasetUpdateStatus.Installed,
                "통계 데이터가 설치되었습니다. 앱을 다시 시작하면 적용됩니다.",
                manifest);
        }
        catch (InvalidDataException exception)
        {
            Debug.WriteLine($"Dataset size guard failure: {exception.GetType().Name}");
            return new(DatasetUpdateStatus.SizeMismatch,
                "다운로드한 데이터의 크기가 업데이트 정보와 다릅니다.");
        }
        catch (JsonException exception)
        {
            Debug.WriteLine($"Dataset JSON failure: {exception.GetType().Name}");
            return new(DatasetUpdateStatus.InvalidDataset,
                "다운로드한 통계 데이터가 올바르지 않아 적용하지 않았습니다.");
        }
        catch (Exception exception) when (exception is HttpRequestException
            or OperationCanceledException or IOException or UnauthorizedAccessException
            or CryptographicException or ObjectDisposedException)
        {
            Debug.WriteLine($"Dataset install failure: {exception.GetType().Name}");
            return new(DatasetUpdateStatus.InstallFailure,
                "업데이트 설치에 실패했습니다. 기존 데이터는 계속 사용할 수 있습니다.");
        }
        finally
        {
            if (datasetReplaced && !manifestReplaced)
            {
                RestoreOrRemove(_datasetPath, datasetBackupPath, datasetExisted);
            }

            TryDelete(temporaryDatasetPath);
            TryDelete(temporaryManifestPath);
            TryDelete(datasetBackupPath);
            TryDelete(manifestBackupPath);
        }
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    private bool TryResolveDownloadUri(
        DatasetUpdateManifest manifest,
        out Uri downloadUri)
    {
        downloadUri = null!;
        if (string.IsNullOrWhiteSpace(manifest.DownloadUrl)
            || _options.ManifestUri is null
            || !Uri.TryCreate(_options.ManifestUri, manifest.DownloadUrl, out var resolved)
            || !IsAllowedRemoteUri(resolved))
        {
            return false;
        }

        downloadUri = resolved;
        return true;
    }

    private static bool ValidateManifest(
        DatasetUpdateManifest? manifest,
        bool requireDownloadUrl) =>
        manifest is
        {
            SchemaVersion: ManifestSchemaVersion,
            DatasetSchemaVersion: StatisticsDatasetValidator.SupportedSchemaVersion,
            DatasetRevision: > 0,
            QueueId: DatasetUpdateOptions.QueueId,
            MinimumGameDurationSeconds: > 0,
            AcceptedMatches: > 0,
            Observations: > 0,
            MatchupKeys: > 0,
            FileSizeBytes: > 0 and <= MaximumDatasetBytes
        }
        && manifest.DatasetId == DatasetUpdateOptions.DatasetId
        && manifest.TierScope == DatasetUpdateOptions.TierScope
        && manifest.Region == DatasetUpdateOptions.Region
        && !string.IsNullOrWhiteSpace(manifest.Patch)
        && manifest.DatasetGeneratedAtUtc != default
        && manifest.PublishedAtUtc != default
        && IsSha256(manifest.Sha256)
        && (!requireDownloadUrl || !string.IsNullOrWhiteSpace(manifest.DownloadUrl));

    private static bool MatchesManifest(
        StatisticsDataset dataset,
        int matchupKeys,
        DatasetUpdateManifest manifest) =>
        dataset.SchemaVersion == manifest.DatasetSchemaVersion
        && dataset.TierScope == manifest.TierScope
        && dataset.Region == manifest.Region
        && dataset.QueueId == manifest.QueueId
        && dataset.Patch == manifest.Patch
        && dataset.MinimumGameDurationSeconds == manifest.MinimumGameDurationSeconds
        && dataset.AcceptedMatches == manifest.AcceptedMatches
        && dataset.Observations == manifest.Observations
        && matchupKeys == manifest.MatchupKeys
        && dataset.GeneratedAtUtc == manifest.DatasetGeneratedAtUtc;

    private static bool HasSameImmutableIdentity(
        DatasetUpdateManifest current,
        DatasetUpdateManifest remote) =>
        current.SchemaVersion == remote.SchemaVersion
        && current.DatasetSchemaVersion == remote.DatasetSchemaVersion
        && current.DatasetId == remote.DatasetId
        && current.DatasetRevision == remote.DatasetRevision
        && current.Patch == remote.Patch
        && current.TierScope == remote.TierScope
        && current.Region == remote.Region
        && current.QueueId == remote.QueueId
        && current.MinimumGameDurationSeconds == remote.MinimumGameDurationSeconds
        && current.AcceptedMatches == remote.AcceptedMatches
        && current.Observations == remote.Observations
        && current.MatchupKeys == remote.MatchupKeys
        && current.DatasetGeneratedAtUtc == remote.DatasetGeneratedAtUtc
        && current.FileSizeBytes == remote.FileSizeBytes
        && string.Equals(current.Sha256, remote.Sha256,
            StringComparison.OrdinalIgnoreCase);

    private static bool IsSha256(string value) => value.Length == 64
        && value.All(character => char.IsAsciiHexDigit(character));

    private static bool IsAllowedRemoteUri(Uri uri)
    {
        if (uri.Scheme == Uri.UriSchemeHttps)
        {
            return true;
        }

#if DEBUG
        return uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback;
#else
        return false;
#endif
    }

    private static async Task<byte[]> ReadLimitedAsync(
        Stream source,
        int maximumBytes,
        CancellationToken cancellationToken)
    {
        using var destination = new MemoryStream();
        await CopyLimitedAsync(source, destination, maximumBytes, cancellationToken);
        return destination.ToArray();
    }

    private static async Task CopyLimitedAsync(
        Stream source,
        Stream destination,
        long maximumBytes,
        CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        long total = 0;
        while (true)
        {
            var read = await source.ReadAsync(buffer, cancellationToken);
            if (read == 0)
            {
                return;
            }

            total += read;
            if (total > maximumBytes)
            {
                throw new InvalidDataException("Downloaded content exceeded its declared size.");
            }

            await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
    }

    private static async Task<string> ComputeSha256Async(
        string path,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static async Task WriteCompleteFileAsync(
        string path,
        byte[] content,
        CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(
            path,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);
        await stream.WriteAsync(content, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }

    private static void ReplaceWithRollback(
        string source,
        string destination,
        string backup,
        bool destinationExists)
    {
        if (destinationExists)
        {
            File.Replace(source, destination, backup, ignoreMetadataErrors: true);
        }
        else
        {
            File.Move(source, destination);
        }
    }

    private void RestoreOrRemove(
        string destination,
        string backup,
        bool destinationExisted)
    {
        try
        {
            if (destinationExisted && File.Exists(backup))
            {
                File.Move(backup, destination, overwrite: true);
            }
            else if (!destinationExisted)
            {
                TryDelete(destination);
            }
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException)
        {
            _logger.Log(AppLogLevel.Error, AppLogEvent.RollbackFailed, exception: exception);
        }
    }

    private void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException)
        {
            _logger.Log(AppLogLevel.Warning, AppLogEvent.CleanupFailed, exception: exception);
        }
    }
}

public enum DatasetUpdateStatus
{
    NotConfigured,
    UpToDate,
    NoDowngrade,
    UpdateAvailable,
    Installed,
    InvalidManifest,
    SizeMismatch,
    HashMismatch,
    InvalidDataset,
    NetworkFailure,
    InstallFailure
}

public sealed record DatasetUpdateCheckResult(
    DatasetUpdateStatus Status,
    string Message,
    DatasetUpdateManifest? Manifest = null);
