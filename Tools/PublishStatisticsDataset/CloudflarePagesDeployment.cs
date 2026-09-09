using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

internal sealed record CloudflarePagesDeploymentResult(
    string DeploymentRoot,
    long Revision,
    string Patch,
    int AcceptedMatches,
    string DatasetFileName,
    long DatasetSizeBytes,
    string DatasetSha256,
    long TotalSizeBytes,
    int TotalFiles,
    string ManifestRelativePath,
    string DatasetRelativePath,
    string HeadersPath,
    string HeadersContent,
    HttpStatusCode ManifestStatus,
    HttpStatusCode DatasetStatus);

internal static class CloudflarePagesDeployment
{
    // Cloudflare Pages Direct Upload accepts individual static files up to 25 MiB.
    private const long CloudflarePagesMaximumFileSizeBytes = 25L * 1024 * 1024;
    private static readonly string[] SecretPatterns =
        ["RGAPI-", "RIOT_API_KEY", "password", "secret", "token"];
    private static readonly HashSet<string> PrivatePropertyNames = new(
        ["puuid", "summonerName", "riotId", "gameName", "tagLine", "matchId"],
        StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> AllowedRelativePaths = new(
        StringComparer.OrdinalIgnoreCase);

    public static async Task<CloudflarePagesDeploymentResult> PrepareAsync(
        string repositoryRoot,
        string publisherOutputRoot,
        string deploymentRoot)
    {
        repositoryRoot = Path.GetFullPath(repositoryRoot);
        publisherOutputRoot = Path.GetFullPath(publisherOutputRoot);
        deploymentRoot = Path.GetFullPath(deploymentRoot);
        EnsureSafeDeploymentRoot(repositoryRoot, deploymentRoot);

        var sourceManifestPath = Path.Combine(publisherOutputRoot, "manifest.json");
        var sourceManifestBytes = await File.ReadAllBytesAsync(sourceManifestPath);
        var manifest = JsonSerializer.Deserialize<DatasetUpdateManifest>(sourceManifestBytes)
            ?? throw new InvalidDataException("Publisher manifest is empty.");
        var datasetFileName = ValidateManifestAndGetDatasetFileName(manifest);
        var sourceDatasetPath = Path.Combine(publisherOutputRoot, datasetFileName);
        if (!File.Exists(sourceDatasetPath))
        {
            throw new FileNotFoundException(
                "The revision-specific dataset referenced by the manifest was not found.",
                sourceDatasetPath);
        }

        var sourceDatasetBytes = await File.ReadAllBytesAsync(sourceDatasetPath);
        ValidateDataset(manifest, sourceDatasetBytes);

        var stagingRoot = deploymentRoot + $".{Guid.NewGuid():N}.tmp";
        var backupRoot = deploymentRoot + $".{Guid.NewGuid():N}.backup";
        var manifestRelativePath = "statistics/master-plus/manifest.json";
        var datasetRelativePath = $"statistics/master-plus/{datasetFileName}";
        var headersContent = CreateHeaders(datasetFileName);
        var finalInstalled = false;

        try
        {
            var targetDirectory = Path.Combine(stagingRoot, "statistics", "master-plus");
            Directory.CreateDirectory(targetDirectory);
            await File.WriteAllBytesAsync(
                Path.Combine(targetDirectory, "manifest.json"), sourceManifestBytes);
            await File.WriteAllBytesAsync(
                Path.Combine(targetDirectory, datasetFileName), sourceDatasetBytes);
            await File.WriteAllTextAsync(
                Path.Combine(stagingRoot, "_headers"),
                headersContent,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            AllowedRelativePaths.Clear();
            AllowedRelativePaths.Add("_headers");
            AllowedRelativePaths.Add(manifestRelativePath);
            AllowedRelativePaths.Add(datasetRelativePath);
            VerifyAllowlist(stagingRoot);
            VerifyFileSizes(stagingRoot);
            VerifyTextScans(stagingRoot);
            VerifyPrivateIdentifiers(sourceManifestBytes, "manifest.json");
            VerifyPrivateIdentifiers(sourceDatasetBytes, datasetFileName);
            VerifyPackagedBytes(stagingRoot, sourceManifestBytes, sourceDatasetBytes, datasetFileName);

            if (Directory.Exists(deploymentRoot))
            {
                Directory.Move(deploymentRoot, backupRoot);
            }

            try
            {
                Directory.Move(stagingRoot, deploymentRoot);
                finalInstalled = true;
            }
            catch
            {
                if (Directory.Exists(backupRoot) && !Directory.Exists(deploymentRoot))
                {
                    Directory.Move(backupRoot, deploymentRoot);
                }

                throw;
            }

            if (Directory.Exists(backupRoot))
            {
                Directory.Delete(backupRoot, recursive: true);
            }

            var smoke = await RunHttpSmokeTestAsync(
                deploymentRoot, manifestRelativePath, datasetRelativePath, manifest.Sha256);
            var files = Directory.GetFiles(deploymentRoot, "*", SearchOption.AllDirectories);
            var totalSize = files.Sum(path => new FileInfo(path).Length);
            return new CloudflarePagesDeploymentResult(
                deploymentRoot,
                manifest.DatasetRevision,
                manifest.Patch,
                manifest.AcceptedMatches,
                datasetFileName,
                sourceDatasetBytes.LongLength,
                manifest.Sha256.ToLowerInvariant(),
                totalSize,
                files.Length,
                manifestRelativePath,
                datasetRelativePath,
                Path.Combine(deploymentRoot, "_headers"),
                headersContent,
                smoke.ManifestStatus,
                smoke.DatasetStatus);
        }
        finally
        {
            if (Directory.Exists(stagingRoot))
            {
                Directory.Delete(stagingRoot, recursive: true);
            }

            if (finalInstalled && Directory.Exists(backupRoot))
            {
                Directory.Delete(backupRoot, recursive: true);
            }
        }
    }

    private static string ValidateManifestAndGetDatasetFileName(DatasetUpdateManifest manifest)
    {
        if (manifest.SchemaVersion != 1
            || manifest.DatasetSchemaVersion != StatisticsDatasetValidator.SupportedSchemaVersion
            || manifest.DatasetId != DatasetUpdateOptions.DatasetId
            || manifest.DatasetRevision <= 0
            || string.IsNullOrWhiteSpace(manifest.Patch)
            || string.IsNullOrWhiteSpace(manifest.TierScope)
            || string.IsNullOrWhiteSpace(manifest.Region)
            || manifest.QueueId != StatisticsDatasetValidator.RankedSoloQueueId
            || manifest.MinimumGameDurationSeconds <= 0
            || manifest.AcceptedMatches <= 0
            || manifest.Observations <= 0
            || manifest.MatchupKeys <= 0
            || manifest.FileSizeBytes <= 0
            || manifest.Sha256.Length != 64
            || !manifest.Sha256.All(Uri.IsHexDigit)
            || string.IsNullOrWhiteSpace(manifest.DownloadUrl))
        {
            throw new InvalidDataException("Publisher manifest metadata is invalid or incomplete.");
        }

        var downloadUrl = manifest.DownloadUrl;
        if (downloadUrl.Contains('\\')
            || !Uri.TryCreate(downloadUrl, UriKind.Relative, out _)
            || downloadUrl.Contains('/')
            || downloadUrl.Contains("..", StringComparison.Ordinal)
            || downloadUrl.Contains("localhost", StringComparison.OrdinalIgnoreCase)
            || downloadUrl.Contains("example.com", StringComparison.OrdinalIgnoreCase)
            || downloadUrl.Contains("placeholder", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                "Publisher manifest downloadUrl must be a safe same-directory relative filename.");
        }

        var expectedFileName = $"master_plus.{manifest.DatasetRevision}.json";
        if (!string.Equals(downloadUrl, expectedFileName, StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                $"Publisher manifest downloadUrl must be {expectedFileName}.");
        }

        return expectedFileName;
    }

    private static void ValidateDataset(DatasetUpdateManifest manifest, byte[] datasetBytes)
    {
        if (datasetBytes.LongLength != manifest.FileSizeBytes)
        {
            throw new InvalidDataException("Publisher dataset size does not match the manifest.");
        }

        var sha256 = Convert.ToHexString(SHA256.HashData(datasetBytes)).ToLowerInvariant();
        if (!string.Equals(sha256, manifest.Sha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("Publisher dataset SHA-256 does not match the manifest.");
        }

        var dataset = JsonSerializer.Deserialize<StatisticsDataset>(datasetBytes);
        if (!StatisticsDatasetValidator.TryValidate(dataset, out var index, out var error))
        {
            throw new InvalidDataException($"Publisher dataset validation failed: {error}");
        }

        if (dataset!.SchemaVersion != manifest.DatasetSchemaVersion
            || dataset.Patch != manifest.Patch
            || dataset.TierScope != manifest.TierScope
            || dataset.Region != manifest.Region
            || dataset.QueueId != manifest.QueueId
            || dataset.MinimumGameDurationSeconds != manifest.MinimumGameDurationSeconds
            || dataset.AcceptedMatches != manifest.AcceptedMatches
            || dataset.Observations != manifest.Observations
            || index.Count != manifest.MatchupKeys
            || dataset.GeneratedAtUtc != manifest.DatasetGeneratedAtUtc)
        {
            throw new InvalidDataException(
                "Publisher dataset metadata does not match the manifest.");
        }
    }

    private static string CreateHeaders(string datasetFileName) =>
        "/statistics/master-plus/manifest.json\n"
        + "  Cache-Control: no-cache, max-age=0, must-revalidate\n\n"
        + $"/statistics/master-plus/{datasetFileName}\n"
        + "  Cache-Control: public, max-age=31536000, immutable\n";

    private static void VerifyAllowlist(string root)
    {
        var actual = Directory.GetFiles(root, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(root, path).Replace('\\', '/'))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!actual.SetEquals(AllowedRelativePaths))
        {
            throw new InvalidDataException(
                $"Deployment allowlist failed. Files: {string.Join(", ", actual.Order())}");
        }
    }

    private static void VerifyFileSizes(string root)
    {
        var oversized = Directory.GetFiles(root, "*", SearchOption.AllDirectories)
            .Select(path => new FileInfo(path))
            .FirstOrDefault(file => file.Length > CloudflarePagesMaximumFileSizeBytes);
        if (oversized is not null)
        {
            throw new InvalidDataException(
                $"Cloudflare Pages Direct Upload incompatible: {oversized.Name} exceeds 25 MiB.");
        }
    }

    private static void VerifyTextScans(string root)
    {
        foreach (var path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(path, Encoding.UTF8);
            var match = SecretPatterns.FirstOrDefault(pattern =>
                text.Contains(pattern, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
            {
                throw new InvalidDataException(
                    $"Potential credential pattern '{match}' found in {Path.GetFileName(path)}.");
            }
        }
    }

    private static void VerifyPrivateIdentifiers(byte[] jsonBytes, string fileName)
    {
        var text = Encoding.UTF8.GetString(jsonBytes);
        var match = PrivatePropertyNames.FirstOrDefault(identifier =>
            text.Contains(identifier, StringComparison.OrdinalIgnoreCase));
        if (match is not null)
        {
            throw new InvalidDataException(
                $"Private identifier '{match}' found in {fileName}.");
        }
    }

    private static void VerifyPackagedBytes(
        string root,
        byte[] sourceManifestBytes,
        byte[] sourceDatasetBytes,
        string datasetFileName)
    {
        var targetDirectory = Path.Combine(root, "statistics", "master-plus");
        if (!File.ReadAllBytes(Path.Combine(targetDirectory, "manifest.json"))
                .SequenceEqual(sourceManifestBytes)
            || !File.ReadAllBytes(Path.Combine(targetDirectory, datasetFileName))
                .SequenceEqual(sourceDatasetBytes))
        {
            throw new InvalidDataException("Deployment artifacts are not byte-for-byte copies.");
        }
    }

    private static void EnsureSafeDeploymentRoot(string repositoryRoot, string deploymentRoot)
    {
        var relative = Path.GetRelativePath(repositoryRoot, deploymentRoot);
        if (relative == "."
            || relative == ".."
            || relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
            || Path.IsPathRooted(relative))
        {
            throw new InvalidOperationException(
                "Deployment output must be a child directory of the repository root.");
        }
    }

    private static async Task<(HttpStatusCode ManifestStatus, HttpStatusCode DatasetStatus)>
        RunHttpSmokeTestAsync(
            string root,
            string manifestRelativePath,
            string datasetRelativePath,
            string expectedSha256)
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var server = ServeFilesAsync(listener, root, requestCount: 2, timeout.Token);
        using var client = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{port}/") };
        try
        {
            using var manifestResponse = await client.GetAsync(manifestRelativePath, timeout.Token);
            using var datasetResponse = await client.GetAsync(datasetRelativePath, timeout.Token);
            var datasetBytes = await datasetResponse.Content.ReadAsByteArrayAsync(timeout.Token);
            var actualSha256 = Convert.ToHexString(SHA256.HashData(datasetBytes)).ToLowerInvariant();
            if (manifestResponse.StatusCode != HttpStatusCode.OK
                || datasetResponse.StatusCode != HttpStatusCode.OK
                || !string.Equals(actualSha256, expectedSha256, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("Local HTTP deployment smoke test failed.");
            }

            await server;
            return (manifestResponse.StatusCode, datasetResponse.StatusCode);
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

    private static async Task ServeFilesAsync(
        TcpListener listener,
        string root,
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

            var requestPath = requestLine?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ElementAtOrDefault(1)?.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var candidate = requestPath is null ? null : Path.GetFullPath(Path.Combine(root, requestPath));
            var relative = candidate is null ? ".." : Path.GetRelativePath(root, candidate);
            var found = candidate is not null
                && relative != ".."
                && !relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                && File.Exists(candidate);
            var content = found ? await File.ReadAllBytesAsync(candidate!, cancellationToken) : [];
            var status = found ? "200 OK" : "404 Not Found";
            var responseHeaders = Encoding.ASCII.GetBytes(
                $"HTTP/1.1 {status}\r\nContent-Type: application/json\r\n"
                + $"Content-Length: {content.Length}\r\nConnection: close\r\n\r\n");
            await stream.WriteAsync(responseHeaders, cancellationToken);
            await stream.WriteAsync(content, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
    }
}
