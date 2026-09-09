using System.Security.Cryptography;
using System.Text.Json;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

if (args.Contains("--self-test", StringComparer.Ordinal))
{
    Environment.ExitCode = await UpdatePipelineSelfTest.RunAsync();
    return;
}

var remoteManifestUrl = GetArgument("--verify-remote");
if (remoteManifestUrl is not null)
{
    Environment.ExitCode = await RemoteDatasetVerifier.RunAsync(remoteManifestUrl);
    return;
}

var repositoryRoot = FindRepositoryRoot();
if (args.Contains("--prepare-cloudflare-pages", StringComparer.Ordinal))
{
    var publisherOutputRoot = Path.GetFullPath(GetArgument("--output") ?? Path.Combine(
        repositoryRoot,
        "Tools",
        "PublishStatisticsDataset",
        "Output",
        "master-plus"));
    var deploymentRoot = Path.GetFullPath(GetArgument("--deployment-output") ?? Path.Combine(
        repositoryRoot,
        "Tools",
        "PublishStatisticsDataset",
        "CloudflarePagesDeploy"));
    try
    {
        var result = await CloudflarePagesDeployment.PrepareAsync(
            repositoryRoot, publisherOutputRoot, deploymentRoot);
        Console.WriteLine("Cloudflare Pages deployment package ready");
        Console.WriteLine($"Deployment root: {result.DeploymentRoot}");
        Console.WriteLine($"Revision: {result.Revision}");
        Console.WriteLine($"Patch: {result.Patch}");
        Console.WriteLine($"Accepted matches: {result.AcceptedMatches:N0}");
        Console.WriteLine($"Dataset filename: {result.DatasetFileName}");
        Console.WriteLine($"File size: {result.DatasetSizeBytes:N0} bytes");
        Console.WriteLine($"SHA-256: {result.DatasetSha256}");
        Console.WriteLine($"Manifest relative path: {result.ManifestRelativePath}");
        Console.WriteLine($"Dataset relative path: {result.DatasetRelativePath}");
        Console.WriteLine($"Headers: {result.HeadersPath}");
        Console.WriteLine($"Total files: {result.TotalFiles}");
        Console.WriteLine($"Total directory size: {result.TotalSizeBytes:N0} bytes");
        Console.WriteLine("Allowlist: PASS");
        Console.WriteLine("Secret scan: PASS");
        Console.WriteLine("Privacy scan: PASS");
        Console.WriteLine("Cloudflare 25 MiB guard: PASS");
        Console.WriteLine($"Local HTTP manifest: {(int)result.ManifestStatus}");
        Console.WriteLine($"Local HTTP dataset: {(int)result.DatasetStatus}");
        Console.WriteLine("Local HTTP SHA-256: PASS");
        Console.WriteLine("Upload this folder to Cloudflare Pages Direct Upload:");
        Console.WriteLine(result.DeploymentRoot);
    }
    catch (Exception exception) when (exception is IOException
        or UnauthorizedAccessException or JsonException or InvalidDataException
        or CryptographicException or ArgumentException or InvalidOperationException
        or HttpRequestException)
    {
        Console.Error.WriteLine($"Deployment package failure: {exception.Message}");
        Environment.ExitCode = 1;
    }

    return;
}

var sourcePath = ResolveSourcePath(repositoryRoot);
var baselineDirectory = Path.Combine(repositoryRoot, "Data", "Statistics");
var baselineDatasetPath = Path.Combine(baselineDirectory, "master_plus.json");
var baselineManifestPath = Path.Combine(baselineDirectory, "manifest.json");
var artifactDirectory = Path.GetFullPath(GetArgument("--output") ?? Path.Combine(
    repositoryRoot,
    "Tools",
    "PublishStatisticsDataset",
    "Output",
    "master-plus"));

Console.WriteLine("LoL Matchup statistics dataset publish");
Console.WriteLine($"Source: {sourcePath}");
Console.WriteLine($"Packaged baseline: {baselineDatasetPath}");
Console.WriteLine($"Remote artifacts: {artifactDirectory}");

try
{
    var sourceBytes = await File.ReadAllBytesAsync(sourcePath);
    var dataset = JsonSerializer.Deserialize<StatisticsDataset>(sourceBytes);
    if (!StatisticsDatasetValidator.TryValidate(dataset, out var index, out var validationError))
    {
        throw new InvalidDataException(validationError);
    }

    var revision = ResolveRevision(dataset!);
    var artifactFileName = $"master_plus.{revision}.json";
    var downloadUrl = GetArgument("--download-url") ?? artifactFileName;
    ValidateDownloadUrl(downloadUrl);
    var sha256 = Convert.ToHexString(SHA256.HashData(sourceBytes)).ToLowerInvariant();
    var manifest = new DatasetUpdateManifest
    {
        SchemaVersion = 1,
        DatasetSchemaVersion = dataset!.SchemaVersion,
        DatasetId = DatasetUpdateOptions.DatasetId,
        DatasetRevision = revision,
        Patch = dataset.Patch,
        TierScope = dataset.TierScope,
        Region = dataset.Region,
        QueueId = dataset.QueueId,
        MinimumGameDurationSeconds = dataset.MinimumGameDurationSeconds!.Value,
        AcceptedMatches = dataset.AcceptedMatches,
        Observations = dataset.Observations,
        MatchupKeys = index.Count,
        DatasetGeneratedAtUtc = dataset.GeneratedAtUtc,
        PublishedAtUtc = DateTimeOffset.UtcNow,
        FileSizeBytes = sourceBytes.LongLength,
        Sha256 = sha256,
        DownloadUrl = downloadUrl
    };
    var manifestBytes = JsonSerializer.SerializeToUtf8Bytes(
        manifest,
        new JsonSerializerOptions { WriteIndented = true });
    var artifactDatasetPath = Path.Combine(artifactDirectory, artifactFileName);

    Directory.CreateDirectory(baselineDirectory);
    Directory.CreateDirectory(artifactDirectory);
    await EnsureImmutableArtifactAsync(artifactDatasetPath, sourceBytes);
    await WriteAtomicallyAsync(baselineDatasetPath, sourceBytes);
    await WriteAtomicallyAsync(baselineManifestPath, manifestBytes);
    if (!File.Exists(artifactDatasetPath))
    {
        await WriteAtomicallyAsync(artifactDatasetPath, sourceBytes);
    }
    await WriteAtomicallyAsync(
        Path.Combine(artifactDirectory, "manifest.json"),
        manifestBytes);

    await VerifyPublishedDatasetAsync(baselineDatasetPath, sourceBytes);
    await VerifyPublishedDatasetAsync(artifactDatasetPath, sourceBytes);

    Console.WriteLine($"Patch: {dataset.Patch}");
    Console.WriteLine($"Accepted matches: {dataset.AcceptedMatches:N0}");
    Console.WriteLine($"Observations: {dataset.Observations:N0}");
    Console.WriteLine($"Matchup keys: {index.Count:N0}");
    Console.WriteLine($"Minimum duration: {dataset.MinimumGameDurationSeconds} seconds");
    Console.WriteLine($"Dataset generated: {dataset.GeneratedAtUtc:O}");
    Console.WriteLine($"Dataset revision: {revision}");
    Console.WriteLine($"File size: {sourceBytes.LongLength:N0} bytes");
    Console.WriteLine($"SHA-256: {sha256}");
    Console.WriteLine($"Remote dataset artifact: {artifactFileName}");
    Console.WriteLine("Publish success");
}
catch (Exception exception) when (exception is IOException
    or UnauthorizedAccessException or JsonException or InvalidDataException
    or CryptographicException or ArgumentException or InvalidOperationException)
{
    Console.Error.WriteLine($"Publish failure: {exception.Message}");
    Environment.ExitCode = 1;
}

string ResolveSourcePath(string root)
{
    var explicitSource = GetArgument("--source");
    if (!string.IsNullOrWhiteSpace(explicitSource))
    {
        return Path.GetFullPath(explicitSource);
    }

    var outputRoot = Path.Combine(
        root,
        "Tools",
        "StatsCollector",
        "Data",
        "Output",
        DatasetUpdateOptions.TierScope,
        DatasetUpdateOptions.Region,
        DatasetUpdateOptions.QueueId.ToString(System.Globalization.CultureInfo.InvariantCulture));
    var patch = GetArgument("--patch")
        ?? Environment.GetEnvironmentVariable("TARGET_PATCH");
    if (!string.IsNullOrWhiteSpace(patch))
    {
        var candidate = Path.Combine(outputRoot, patch, "MIN900S", "matchups.json");
        if (!File.Exists(candidate))
        {
            throw new FileNotFoundException(
                $"Collector output was not found for patch {patch}.",
                candidate);
        }

        return candidate;
    }

    var candidates = Directory.Exists(outputRoot)
        ? Directory.GetFiles(outputRoot, "matchups.json", SearchOption.AllDirectories)
            .Where(path => string.Equals(
                new DirectoryInfo(Path.GetDirectoryName(path)!).Name,
                "MIN900S",
                StringComparison.Ordinal))
            .ToArray()
        : [];
    return candidates.Length switch
    {
        1 => candidates[0],
        0 => throw new FileNotFoundException(
            "No MIN900S collector output was found. Use --source or --patch."),
        _ => throw new InvalidOperationException(
            "Multiple collector outputs were found. Use --source or --patch explicitly.")
    };
}

long ResolveRevision(StatisticsDataset dataset)
{
    var value = GetArgument("--revision");
    if (value is not null)
    {
        return long.TryParse(value, out var explicitRevision) && explicitRevision > 0
            ? explicitRevision
            : throw new ArgumentException("--revision must be a positive integer.");
    }

    var revision = dataset.GeneratedAtUtc.ToUnixTimeMilliseconds();
    return revision > 0
        ? revision
        : throw new InvalidDataException(
            "Dataset generatedAtUtc cannot produce a valid revision.");
}

string? GetArgument(string name)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

static void ValidateDownloadUrl(string value)
{
    if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
    {
        throw new ArgumentException("--download-url is not a valid URI.");
    }

    if (uri.IsAbsoluteUri && uri.Scheme != Uri.UriSchemeHttps)
    {
        throw new ArgumentException("An absolute --download-url must use HTTPS.");
    }
}

static string FindRepositoryRoot()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory is not null
        && !File.Exists(Path.Combine(directory.FullName, "LoLMatchup.csproj")))
    {
        directory = directory.Parent;
    }

    return directory?.FullName
        ?? throw new DirectoryNotFoundException(
            "LoLMatchup repository root was not found.");
}

static async Task WriteAtomicallyAsync(string destination, byte[] content)
{
    var temporary = destination + $".{Environment.ProcessId}.{Guid.NewGuid():N}.tmp";
    try
    {
        await using (var stream = new FileStream(
            temporary,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true))
        {
            await stream.WriteAsync(content);
            await stream.FlushAsync();
        }

        File.Move(temporary, destination, overwrite: true);
    }
    finally
    {
        if (File.Exists(temporary))
        {
            File.Delete(temporary);
        }
    }
}

static async Task VerifyPublishedDatasetAsync(string path, byte[] sourceBytes)
{
    var publishedBytes = await File.ReadAllBytesAsync(path);
    var published = JsonSerializer.Deserialize<StatisticsDataset>(publishedBytes);
    if (!StatisticsDatasetValidator.TryValidate(published, out _, out _)
        || !SHA256.HashData(publishedBytes).SequenceEqual(SHA256.HashData(sourceBytes)))
    {
        throw new InvalidDataException("Published dataset verification failed.");
    }
}

static async Task EnsureImmutableArtifactAsync(string path, byte[] sourceBytes)
{
    if (!File.Exists(path))
    {
        return;
    }

    var existingBytes = await File.ReadAllBytesAsync(path);
    if (!SHA256.HashData(existingBytes).SequenceEqual(SHA256.HashData(sourceBytes)))
    {
        throw new InvalidDataException(
            "The dataset revision already exists with different content. "
            + "Use a corrected source revision instead of overwriting an immutable artifact.");
    }
}
