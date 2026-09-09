using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services;
using LoLMatchup.Services.Statistics;

internal static class UpdatePipelineSelfTest
{
    private static readonly Uri ManifestUri = new("http://127.0.0.1:18080/manifest.json");

    public static async Task<int> RunAsync()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            $"lolmatchup-update-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);

        try
        {
            var currentBytes = CreateDatasetBytes(1, DateTimeOffset.Parse("2026-09-01T00:00:00Z"));
            var updateBytes = CreateDatasetBytes(2, DateTimeOffset.Parse("2026-09-02T00:00:00Z"));
            var currentRevision = 100L;
            var updateManifest = CreateManifest(updateBytes, 200);
            var currentManifest = CreateManifest(currentBytes, currentRevision);

            var results = new Dictionary<string, bool>
            {
                ["production endpoint/default override precedence"] =
                    VerifyEndpointConfiguration(),
                ["non-HTTPS remote rejected"] = await VerifyUnsafeManifestEndpointAsync(
                    root, currentBytes),
                ["latest"] = await CheckStatusAsync(
                    root, currentBytes, currentManifest, currentRevision,
                    DatasetUpdateStatus.UpToDate),
                ["same revision conflict"] = await CheckManifestIdentityAsync(
                    root, currentBytes, currentManifest,
                    Copy(currentManifest, sha256: new string('0', 64)),
                    DatasetUpdateStatus.InvalidManifest),
                ["newer/install"] = await InstallAndVerifyAsync(
                    root, currentBytes, updateBytes, updateManifest),
                ["loopback HTTP install"] = await VerifyLoopbackHttpInstallAsync(
                    root, currentBytes, updateBytes),
                ["sha mismatch"] = await RejectAndPreserveAsync(
                    root, currentBytes, updateBytes,
                    Copy(updateManifest, sha256: new string('0', 64)),
                    DatasetUpdateStatus.HashMismatch),
                ["size mismatch"] = await RejectAndPreserveAsync(
                    root, currentBytes, updateBytes,
                    Copy(updateManifest, fileSize: updateBytes.LongLength + 1),
                    DatasetUpdateStatus.SizeMismatch),
                ["malformed manifest"] = await CheckRawManifestAsync(
                    root, currentBytes, Encoding.UTF8.GetBytes("{"),
                    currentRevision, DatasetUpdateStatus.InvalidManifest),
                ["invalid dataset"] = await RejectInvalidDatasetAsync(root, currentBytes),
                ["metadata mismatch"] = await RejectAndPreserveAsync(
                    root, currentBytes, updateBytes,
                    Copy(updateManifest, acceptedMatches: updateManifest.AcceptedMatches + 1),
                    DatasetUpdateStatus.InvalidDataset),
                ["unsafe dataset URI"] = await CheckStatusAsync(
                    root, currentBytes,
                    Copy(updateManifest, downloadUrl: "file:///temporary/master_plus.json"),
                    currentRevision, DatasetUpdateStatus.InvalidManifest),
                ["older/no downgrade"] = await CheckStatusAsync(
                    root, currentBytes, CreateManifest(currentBytes, 50), currentRevision,
                    DatasetUpdateStatus.NoDowngrade),
                ["network failure"] = await CheckNetworkFailureAsync(root, currentBytes),
                ["interrupted download"] = await RejectInterruptedDownloadAsync(
                    root, currentBytes, updateBytes, updateManifest),
                ["cancel/dispose during manifest body (3 cycles)"] = await VerifyCancellationAsync(
                    root, currentBytes, updateBytes, updateManifest, install: false),
                ["cancel/dispose during dataset body (3 cycles)"] = await VerifyCancellationAsync(
                    root, currentBytes, updateBytes, updateManifest, install: true),
                ["atomic replacement"] = await VerifyAtomicRollbackAsync(
                    root, currentBytes, updateBytes, updateManifest, currentManifest),
                ["packaged fallback"] = await VerifyPackagedFallbackAsync(root, currentBytes)
            };

            Console.WriteLine("Dataset update pipeline self-test");
            foreach (var result in results)
            {
                Console.WriteLine($"{result.Key}: {(result.Value ? "PASS" : "FAIL")}");
            }

            return results.Values.All(value => value) ? 0 : 1;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Self-test failed: {exception.Message}");
            return 1;
        }
        finally
        {
            try
            {
                Directory.Delete(root, recursive: true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }

    private static async Task<bool> CheckStatusAsync(
        string root,
        byte[] currentBytes,
        DatasetUpdateManifest remoteManifest,
        long currentRevision,
        DatasetUpdateStatus expected)
    {
        var path = PrepareScenario(root, currentBytes);
        using var service = CreateService(path, remoteManifest, currentBytes);
        var result = await service.CheckAsync(currentRevision);
        return result.Status == expected && File.ReadAllBytes(path).SequenceEqual(currentBytes);
    }

    private static bool VerifyEndpointConfiguration()
    {
        var variable = DatasetUpdateOptions.ManifestUrlEnvironmentVariableName;
        var original = Environment.GetEnvironmentVariable(variable);
        try
        {
            Environment.SetEnvironmentVariable(variable, null);
            var defaultOptions = DatasetUpdateOptions.FromEnvironment();
            var overrideUri = new Uri("http://127.0.0.1:18081/manifest.json");
            Environment.SetEnvironmentVariable(variable, overrideUri.AbsoluteUri);
            var overrideOptions = DatasetUpdateOptions.FromEnvironment();
            Environment.SetEnvironmentVariable(variable, "not an absolute URI");
            var invalidOptions = DatasetUpdateOptions.FromEnvironment();
            return defaultOptions.ManifestUri == DatasetUpdateOptions.ProductionManifestUri
                && overrideOptions.ManifestUri == overrideUri
                && invalidOptions.ManifestUri is null;
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, original);
        }
    }

    private static async Task<bool> VerifyUnsafeManifestEndpointAsync(
        string root,
        byte[] currentBytes)
    {
        var path = PrepareScenario(root, currentBytes);
        using var service = new DatasetUpdateService(
            new DatasetUpdateOptions
            {
                ManifestUri = new Uri("http://example.invalid/manifest.json")
            },
            path,
            new HttpClient(new StubHandler(_ =>
                throw new InvalidOperationException("Unsafe endpoint must not be requested."))));
        var result = await service.CheckAsync(currentRevision: 0);
        return result.Status == DatasetUpdateStatus.InvalidManifest
            && File.ReadAllBytes(path).SequenceEqual(currentBytes);
    }

    private static async Task<bool> CheckManifestIdentityAsync(
        string root,
        byte[] currentBytes,
        DatasetUpdateManifest currentManifest,
        DatasetUpdateManifest remoteManifest,
        DatasetUpdateStatus expected)
    {
        var path = PrepareScenario(root, currentBytes);
        using var service = CreateService(path, remoteManifest, currentBytes);
        var result = await service.CheckAsync(currentManifest);
        return result.Status == expected && File.ReadAllBytes(path).SequenceEqual(currentBytes);
    }

    private static async Task<bool> CheckRawManifestAsync(
        string root,
        byte[] currentBytes,
        byte[] manifestBytes,
        long currentRevision,
        DatasetUpdateStatus expected)
    {
        var path = PrepareScenario(root, currentBytes);
        using var client = new HttpClient(new StubHandler(request =>
            request.RequestUri == ManifestUri
                ? Response(manifestBytes, "application/json")
                : new HttpResponseMessage(HttpStatusCode.NotFound)));
        using var service = CreateService(path, client);
        var result = await service.CheckAsync(currentRevision);
        return result.Status == expected && File.ReadAllBytes(path).SequenceEqual(currentBytes);
    }

    private static async Task<bool> InstallAndVerifyAsync(
        string root,
        byte[] currentBytes,
        byte[] updateBytes,
        DatasetUpdateManifest manifest)
    {
        var path = PrepareScenario(root, currentBytes);
        using var service = CreateService(path, manifest, updateBytes);
        var check = await service.CheckAsync(100);
        var install = check.Manifest is null
            ? null
            : await service.InstallAsync(check.Manifest);
        var companionPath = Path.Combine(Path.GetDirectoryName(path)!, "manifest.json");
        return check.Status == DatasetUpdateStatus.UpdateAvailable
            && install?.Status == DatasetUpdateStatus.Installed
            && File.ReadAllBytes(path).SequenceEqual(updateBytes)
            && File.Exists(companionPath)
            && !Directory.GetFiles(Path.GetDirectoryName(path)!, "*.rollback").Any()
            && !Directory.GetFiles(Path.GetDirectoryName(path)!, "*.download").Any();
    }

    private static async Task<bool> VerifyLoopbackHttpInstallAsync(
        string root,
        byte[] currentBytes,
        byte[] updateBytes)
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var endpoint = (IPEndPoint)listener.LocalEndpoint;
        var manifestUri = new Uri(
            $"http://127.0.0.1:{endpoint.Port}/statistics/master-plus/manifest.json");
        var manifest = CreateManifest(updateBytes, 200);
        var manifestBytes = JsonSerializer.SerializeToUtf8Bytes(manifest);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var server = ServeStaticFilesAsync(
            listener,
            new Dictionary<string, byte[]>(StringComparer.Ordinal)
            {
                ["/statistics/master-plus/manifest.json"] = manifestBytes,
                ["/statistics/master-plus/master_plus.200.json"] = updateBytes
            },
            requestCount: 2,
            timeout.Token);

        try
        {
            var path = PrepareScenario(root, currentBytes);
            using var service = new DatasetUpdateService(
                new DatasetUpdateOptions { ManifestUri = manifestUri },
                path);
            var check = await service.CheckAsync(100, timeout.Token);
            var install = check.Manifest is null
                ? null
                : await service.InstallAsync(check.Manifest, timeout.Token);
            await server;
            return check.Status == DatasetUpdateStatus.UpdateAvailable
                && install?.Status == DatasetUpdateStatus.Installed
                && File.ReadAllBytes(path).SequenceEqual(updateBytes);
        }
        finally
        {
            await timeout.CancelAsync();
            listener.Stop();
            try
            {
                await server;
            }
            catch (Exception exception) when (exception is OperationCanceledException
                or ObjectDisposedException or SocketException)
            {
            }
        }
    }

    private static async Task<bool> RejectAndPreserveAsync(
        string root,
        byte[] currentBytes,
        byte[] remoteBytes,
        DatasetUpdateManifest manifest,
        DatasetUpdateStatus expected)
    {
        var path = PrepareScenario(root, currentBytes);
        using var service = CreateService(path, manifest, remoteBytes);
        var result = await service.InstallAsync(manifest);
        return result.Status == expected && File.ReadAllBytes(path).SequenceEqual(currentBytes);
    }

    private static async Task<bool> RejectInvalidDatasetAsync(string root, byte[] currentBytes)
    {
        var invalidBytes = CreateDatasetBytes(2, DateTimeOffset.Parse("2026-09-02T00:00:00Z"),
            source: string.Empty);
        var manifest = CreateManifest(invalidBytes, 200);
        var path = PrepareScenario(root, currentBytes);
        using var service = CreateService(path, manifest, invalidBytes);
        var result = await service.InstallAsync(manifest);
        return result.Status == DatasetUpdateStatus.InvalidDataset
            && File.ReadAllBytes(path).SequenceEqual(currentBytes);
    }

    private static async Task<bool> CheckNetworkFailureAsync(string root, byte[] currentBytes)
    {
        var path = PrepareScenario(root, currentBytes);
        using var client = new HttpClient(new StubHandler(_ =>
            throw new HttpRequestException("Synthetic network failure.")));
        using var service = CreateService(path, client);
        var result = await service.CheckAsync(100);
        return result.Status == DatasetUpdateStatus.NetworkFailure
            && File.ReadAllBytes(path).SequenceEqual(currentBytes);
    }

    private static async Task<bool> RejectInterruptedDownloadAsync(
        string root,
        byte[] currentBytes,
        byte[] updateBytes,
        DatasetUpdateManifest manifest)
    {
        var path = PrepareScenario(root, currentBytes);
        var manifestBytes = JsonSerializer.SerializeToUtf8Bytes(manifest);
        var datasetUri = new Uri(ManifestUri, manifest.DownloadUrl!);
        using var client = new HttpClient(new StubHandler(request =>
        {
            if (request.RequestUri == ManifestUri)
            {
                return Response(manifestBytes, "application/json");
            }

            if (request.RequestUri != datasetUri)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var content = new StreamContent(new InterruptedReadStream(updateBytes));
            content.Headers.ContentLength = updateBytes.LongLength;
            content.Headers.ContentType = new("application/json");
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        }));
        using var service = CreateService(path, client);
        var result = await service.InstallAsync(manifest);
        return result.Status == DatasetUpdateStatus.InstallFailure
            && File.ReadAllBytes(path).SequenceEqual(currentBytes);
    }

    // Mirrors window lifetime cancellation/disposal, using isolated paths, not user data.
    private static async Task<bool> VerifyCancellationAsync(
        string root, byte[] currentBytes, byte[] updateBytes,
        DatasetUpdateManifest manifest, bool install)
    {
        for (var cycle = 0; cycle < 3; cycle++)
        {
            var path = PrepareScenario(root, currentBytes);
            var directory = Path.GetDirectoryName(path)!;
            var companionPath = Path.Combine(directory, "manifest.json");
            var companionBytes = JsonSerializer.SerializeToUtf8Bytes(CreateManifest(currentBytes, 100));
            await File.WriteAllBytesAsync(companionPath, companionBytes);
            using var body = new PausedReadStream(install
                ? updateBytes : JsonSerializer.SerializeToUtf8Bytes(manifest));
            using var client = new HttpClient(new StubHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(body) }));
            using var service = CreateService(path, client);
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var operation = install ? service.InstallAsync(manifest, cancellation.Token)
                : service.CheckAsync(100, cancellation.Token);
            await body.Paused.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await cancellation.CancelAsync();
            service.Dispose();
            client.Dispose();
            var result = await operation.WaitAsync(TimeSpan.FromSeconds(5));
            if (result.Status != (install ? DatasetUpdateStatus.InstallFailure : DatasetUpdateStatus.NetworkFailure)
                || !File.ReadAllBytes(path).SequenceEqual(currentBytes)
                || !File.ReadAllBytes(companionPath).SequenceEqual(companionBytes)
                || Directory.GetFiles(directory).Length != 2)
                return false;
        }
        return true;
    }

    private static async Task<bool> VerifyAtomicRollbackAsync(
        string root,
        byte[] currentBytes,
        byte[] updateBytes,
        DatasetUpdateManifest updateManifest,
        DatasetUpdateManifest currentManifest)
    {
        var path = PrepareScenario(root, currentBytes);
        var companionPath = Path.Combine(Path.GetDirectoryName(path)!, "manifest.json");
        var currentManifestBytes = JsonSerializer.SerializeToUtf8Bytes(currentManifest);
        await File.WriteAllBytesAsync(companionPath, currentManifestBytes);

        using var manifestLock = new FileStream(
            companionPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);
        using var service = CreateService(path, updateManifest, updateBytes);
        var result = await service.InstallAsync(updateManifest);
        return result.Status == DatasetUpdateStatus.InstallFailure
            && File.ReadAllBytes(path).SequenceEqual(currentBytes)
            && File.ReadAllBytes(companionPath).SequenceEqual(currentManifestBytes);
    }

    private static async Task<bool> VerifyPackagedFallbackAsync(string root, byte[] packagedBytes)
    {
        var directory = Path.Combine(root, $"fallback-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var packagedPath = Path.Combine(directory, "packaged.json");
        var localPath = Path.Combine(directory, "local.json");
        await File.WriteAllBytesAsync(packagedPath, packagedBytes);
        await File.WriteAllTextAsync(localPath, "{");
        var service = new StatisticsDataService(packagedPath, localPath);
        var result = await service.LoadAsync();
        return result.Status == StatisticsDatasetLoadStatus.Loaded
            && result.Source == StatisticsDatasetSource.Packaged;
    }

    private static DatasetUpdateService CreateService(
        string datasetPath,
        DatasetUpdateManifest manifest,
        byte[] datasetBytes)
    {
        var manifestBytes = JsonSerializer.SerializeToUtf8Bytes(manifest);
        var datasetUri = new Uri(ManifestUri, manifest.DownloadUrl);
        var client = new HttpClient(new StubHandler(request => request.RequestUri switch
        {
            var uri when uri == ManifestUri => Response(manifestBytes, "application/json"),
            var uri when uri == datasetUri => Response(datasetBytes, "application/json"),
            _ => new HttpResponseMessage(HttpStatusCode.NotFound)
        }));
        return CreateService(datasetPath, client);
    }

    private static DatasetUpdateService CreateService(string datasetPath, HttpClient client) =>
        new(
            new DatasetUpdateOptions { ManifestUri = ManifestUri },
            datasetPath,
            client);

    private static string PrepareScenario(string root, byte[] currentBytes)
    {
        var directory = Path.Combine(root, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "master_plus.json");
        File.WriteAllBytes(path, currentBytes);
        return path;
    }

    private static byte[] CreateDatasetBytes(
        int sampleSize,
        DateTimeOffset generatedAtUtc,
        string source = "SELF_RIOT")
    {
        var dataset = new StatisticsDataset
        {
            SchemaVersion = 1,
            Source = source,
            TierScope = DatasetUpdateOptions.TierScope,
            TierScopeMethod = "SEED_PLAYER_TIER_AT_COLLECTION",
            Region = DatasetUpdateOptions.Region,
            QueueId = 420,
            Patch = "16.17",
            MinimumGameDurationSeconds = 900,
            DurationFilterPolicy = "GAME_DURATION_AT_LEAST_THRESHOLD_SECONDS",
            PolicyKey = "MIN900S",
            GeneratedAtUtc = generatedAtUtc,
            AcceptedMatches = sampleSize,
            Observations = sampleSize * 5,
            Matchups =
            [
                Entry("TOP", "Aatrox", "Akali", sampleSize),
                Entry("JUNGLE", "LeeSin", "Viego", sampleSize),
                Entry("MID", "Ahri", "Syndra", sampleSize),
                Entry("ADC", "Jinx", "Kaisa", sampleSize),
                Entry("SUPPORT", "Lulu", "Thresh", sampleSize)
            ]
        };
        return JsonSerializer.SerializeToUtf8Bytes(dataset);
    }

    private static StatisticsMatchupEntry Entry(
        string role,
        string championA,
        string championB,
        int sampleSize) => new()
        {
            Role = role,
            ChampionA = championA,
            ChampionB = championB,
            SampleSize = sampleSize,
            WinsForA = sampleSize / 2,
            FirstLevel2ForA = sampleSize,
            FirstLevel3ForA = sampleSize,
            FirstLevel6ForA = sampleSize
        };

    private static DatasetUpdateManifest CreateManifest(byte[] bytes, long revision)
    {
        var dataset = JsonSerializer.Deserialize<StatisticsDataset>(bytes)!;
        return new DatasetUpdateManifest
        {
            SchemaVersion = 1,
            DatasetSchemaVersion = dataset.SchemaVersion,
            DatasetId = DatasetUpdateOptions.DatasetId,
            DatasetRevision = revision,
            Patch = dataset.Patch,
            TierScope = dataset.TierScope,
            Region = dataset.Region,
            QueueId = dataset.QueueId,
            MinimumGameDurationSeconds = dataset.MinimumGameDurationSeconds!.Value,
            AcceptedMatches = dataset.AcceptedMatches,
            Observations = dataset.Observations,
            MatchupKeys = dataset.Matchups!.Count,
            DatasetGeneratedAtUtc = dataset.GeneratedAtUtc,
            PublishedAtUtc = DateTimeOffset.Parse("2026-09-03T00:00:00Z"),
            FileSizeBytes = bytes.LongLength,
            Sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),
            DownloadUrl = "master_plus.200.json"
        };
    }

    private static DatasetUpdateManifest Copy(
        DatasetUpdateManifest manifest,
        string? sha256 = null,
        long? fileSize = null,
        int? acceptedMatches = null,
        string? downloadUrl = null) => new()
        {
            SchemaVersion = manifest.SchemaVersion,
            DatasetSchemaVersion = manifest.DatasetSchemaVersion,
            DatasetId = manifest.DatasetId,
            DatasetRevision = manifest.DatasetRevision,
            Patch = manifest.Patch,
            TierScope = manifest.TierScope,
            Region = manifest.Region,
            QueueId = manifest.QueueId,
            MinimumGameDurationSeconds = manifest.MinimumGameDurationSeconds,
            AcceptedMatches = acceptedMatches ?? manifest.AcceptedMatches,
            Observations = manifest.Observations,
            MatchupKeys = manifest.MatchupKeys,
            DatasetGeneratedAtUtc = manifest.DatasetGeneratedAtUtc,
            PublishedAtUtc = manifest.PublishedAtUtc,
            FileSizeBytes = fileSize ?? manifest.FileSizeBytes,
            Sha256 = sha256 ?? manifest.Sha256,
            DownloadUrl = downloadUrl ?? manifest.DownloadUrl
        };

    private static HttpResponseMessage Response(byte[] bytes, string mediaType) => new(HttpStatusCode.OK)
    {
        Content = new ByteArrayContent(bytes)
        {
            Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType) }
        }
    };

    private static async Task ServeStaticFilesAsync(
        TcpListener listener,
        IReadOnlyDictionary<string, byte[]> files,
        int requestCount,
        CancellationToken cancellationToken)
    {
        for (var requestIndex = 0; requestIndex < requestCount; requestIndex++)
        {
            using var client = await listener.AcceptTcpClientAsync(cancellationToken);
            await using var stream = client.GetStream();
            using var reader = new StreamReader(
                stream,
                Encoding.ASCII,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);
            var requestLine = await reader.ReadLineAsync(cancellationToken);
            string? headerLine;
            do
            {
                headerLine = await reader.ReadLineAsync(cancellationToken);
            }
            while (!string.IsNullOrEmpty(headerLine));

            var path = requestLine?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ElementAtOrDefault(1);
            var found = path is not null && files.ContainsKey(path);
            var content = found ? files[path!] : [];
            var status = found ? "200 OK" : "404 Not Found";
            var header = Encoding.ASCII.GetBytes(
                $"HTTP/1.1 {status}\r\nContent-Type: application/json\r\n"
                + $"Content-Length: {content.Length}\r\nConnection: close\r\n\r\n");
            await stream.WriteAsync(header, cancellationToken);
            await stream.WriteAsync(content, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
    }

    private sealed class StubHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => Task.FromResult(responder(request));
    }

    private sealed class PausedReadStream(byte[] bytes) : MemoryStream(bytes)
    {
        public TaskCompletionSource Paused { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (Position == 0)
                return await base.ReadAsync(buffer[..Math.Min(buffer.Length, 16)], cancellationToken);

            var canceled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            using var registration = cancellationToken.Register(() => canceled.TrySetResult());
            Paused.TrySetResult();
            await canceled.Task;
            // Stream APIs may throw the base OperationCanceledException, not TaskCanceledException.
            cancellationToken.ThrowIfCancellationRequested();
            return 0;
        }
    }

    private sealed class InterruptedReadStream(byte[] bytes) : MemoryStream(bytes)
    {
        private readonly long _failurePosition = Math.Max(1, bytes.LongLength / 2);

        public override ValueTask<int> ReadAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            if (Position >= _failurePosition)
            {
                return ValueTask.FromException<int>(
                    new IOException("Synthetic interrupted download."));
            }

            var limitedLength = (int)Math.Min(buffer.Length, _failurePosition - Position);
            return base.ReadAsync(buffer[..limitedLength], cancellationToken);
        }
    }
}
