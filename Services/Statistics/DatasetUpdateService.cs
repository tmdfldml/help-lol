using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Statistics;

public sealed class DatasetUpdateService : IDisposable
{
    private const int ManifestSchemaVersion = 1;
    private readonly DatasetUpdateOptions _options;
    private readonly HttpClient _httpClient;
    private readonly string _datasetPath;
    private readonly string _manifestPath;
    private readonly bool _ownsHttpClient;

    public DatasetUpdateService(
        DatasetUpdateOptions options,
        string datasetPath,
        HttpClient? httpClient = null)
    {
        _options = options;
        _datasetPath = datasetPath;
        _manifestPath = Path.Combine(Path.GetDirectoryName(datasetPath)!, "manifest.json");
        _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        _ownsHttpClient = httpClient is null;
    }

    public async Task<DatasetUpdateCheckResult> CheckAsync(
        long currentRevision,
        CancellationToken cancellationToken = default)
    {
        if (_options.ManifestUri is null)
        {
            return new(DatasetUpdateStatus.NotConfigured, "업데이트 서버가 아직 설정되지 않았습니다.");
        }

        if (_options.ManifestUri.Scheme != Uri.UriSchemeHttps)
        {
            return new(DatasetUpdateStatus.InvalidManifest, "업데이트 서버 주소가 안전하지 않습니다.");
        }

        try
        {
            using var response = await _httpClient.GetAsync(_options.ManifestUri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new(DatasetUpdateStatus.NetworkFailure,
                    $"업데이트 서버 응답 오류 ({(int)response.StatusCode})");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var manifest = await JsonSerializer.DeserializeAsync<DatasetUpdateManifest>(stream, cancellationToken: cancellationToken);
            if (!ValidateManifest(manifest, requireDownloadUrl: true))
            {
                return new(DatasetUpdateStatus.InvalidManifest, "업데이트 정보가 올바르지 않습니다.");
            }

            return manifest!.DatasetRevision > currentRevision
                ? new(DatasetUpdateStatus.UpdateAvailable, "새 데이터가 있습니다.", manifest)
                : new(DatasetUpdateStatus.UpToDate, "최신 데이터입니다.", manifest);
        }
        catch (Exception exception) when (exception is HttpRequestException
            or TaskCanceledException or IOException or JsonException)
        {
            return new(DatasetUpdateStatus.NetworkFailure, "업데이트 확인에 실패했습니다.");
        }
    }

    public async Task<DatasetUpdateCheckResult> InstallAsync(
        DatasetUpdateManifest manifest,
        CancellationToken cancellationToken = default)
    {
        if (!ValidateManifest(manifest, requireDownloadUrl: true)
            || !Uri.TryCreate(manifest.DownloadUrl, UriKind.Absolute, out var downloadUri)
            || downloadUri.Scheme != Uri.UriSchemeHttps)
        {
            return new(DatasetUpdateStatus.InvalidManifest, "업데이트 정보가 올바르지 않습니다.");
        }

        var directory = Path.GetDirectoryName(_datasetPath)!;
        var temporaryDatasetPath = _datasetPath + ".download";
        var temporaryManifestPath = _manifestPath + ".tmp";
        var datasetBackupPath = _datasetPath + ".bak";
        var manifestBackupPath = _manifestPath + ".bak";
        var datasetInstalled = false;
        try
        {
            Directory.CreateDirectory(directory);
            using var response = await _httpClient.GetAsync(downloadUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new(DatasetUpdateStatus.NetworkFailure,
                    $"데이터 다운로드 오류 ({(int)response.StatusCode})");
            }

            await using (var source = await response.Content.ReadAsStreamAsync(cancellationToken))
            await using (var destination = new FileStream(temporaryDatasetPath, FileMode.Create,
                FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await source.CopyToAsync(destination, cancellationToken);
                await destination.FlushAsync(cancellationToken);
            }

            var fileInfo = new FileInfo(temporaryDatasetPath);
            var hash = await ComputeSha256Async(temporaryDatasetPath, cancellationToken);
            if (fileInfo.Length != manifest.FileSizeBytes
                || !string.Equals(hash, manifest.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                return new(DatasetUpdateStatus.HashMismatch, "다운로드한 데이터 검증에 실패했습니다.");
            }

            await using (var stream = File.OpenRead(temporaryDatasetPath))
            {
                var dataset = await JsonSerializer.DeserializeAsync<StatisticsDataset>(stream, cancellationToken: cancellationToken);
                if (!StatisticsDatasetValidator.TryValidate(dataset, out _, out _)
                    || !MatchesManifest(dataset!, manifest))
                {
                    return new(DatasetUpdateStatus.InvalidDataset, "다운로드한 통계 데이터가 올바르지 않습니다.");
                }
            }

            await File.WriteAllTextAsync(temporaryManifestPath,
                JsonSerializer.Serialize(manifest), cancellationToken);
            if (File.Exists(_datasetPath))
            {
                File.Copy(_datasetPath, datasetBackupPath, overwrite: true);
            }
            if (File.Exists(_manifestPath))
            {
                File.Copy(_manifestPath, manifestBackupPath, overwrite: true);
            }
            File.Move(temporaryDatasetPath, _datasetPath, overwrite: true);
            datasetInstalled = true;
            File.Move(temporaryManifestPath, _manifestPath, overwrite: true);
            return new(DatasetUpdateStatus.Installed,
                "업데이트가 완료되었습니다. 앱을 다시 실행하면 새 데이터가 적용됩니다.", manifest);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException
            or IOException or UnauthorizedAccessException or JsonException or CryptographicException)
        {
            if (datasetInstalled)
            {
                RestoreOrRemove(_datasetPath, datasetBackupPath);
                RestoreOrRemove(_manifestPath, manifestBackupPath);
            }
            return new(DatasetUpdateStatus.InstallFailure, "업데이트 설치에 실패했습니다.");
        }
        finally
        {
            TryDelete(temporaryDatasetPath);
            TryDelete(temporaryManifestPath);
        }
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    private static bool ValidateManifest(DatasetUpdateManifest? manifest, bool requireDownloadUrl) =>
        manifest is { SchemaVersion: ManifestSchemaVersion, DatasetRevision: > 0,
            QueueId: DatasetUpdateOptions.QueueId, MinimumGameDurationSeconds: > 0,
            AcceptedMatches: > 0, FileSizeBytes: > 0 }
        && manifest.DatasetId == DatasetUpdateOptions.DatasetId
        && manifest.TierScope == DatasetUpdateOptions.TierScope
        && manifest.Region == DatasetUpdateOptions.Region
        && !string.IsNullOrWhiteSpace(manifest.Patch)
        && manifest.Sha256.Length == 64
        && (!requireDownloadUrl || Uri.TryCreate(manifest.DownloadUrl, UriKind.Absolute, out _));

    private static bool MatchesManifest(StatisticsDataset dataset, DatasetUpdateManifest manifest) =>
        dataset.SchemaVersion == manifest.SchemaVersion
        && dataset.TierScope == manifest.TierScope
        && dataset.Region == manifest.Region
        && dataset.QueueId == manifest.QueueId
        && dataset.Patch == manifest.Patch
        && dataset.MinimumGameDurationSeconds == manifest.MinimumGameDurationSeconds
        && dataset.AcceptedMatches == manifest.AcceptedMatches;

    private static async Task<string> ComputeSha256Async(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) { }
    }

    private static void RestoreOrRemove(string destination, string backup)
    {
        try
        {
            if (File.Exists(backup))
            {
                File.Copy(backup, destination, overwrite: true);
            }
            else
            {
                TryDelete(destination);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) { }
    }
}

public enum DatasetUpdateStatus
{
    NotConfigured, UpToDate, UpdateAvailable, Installed, InvalidManifest,
    HashMismatch, InvalidDataset, NetworkFailure, InstallFailure
}

public sealed record DatasetUpdateCheckResult(
    DatasetUpdateStatus Status,
    string Message,
    DatasetUpdateManifest? Manifest = null);
