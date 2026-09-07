using System.Security.Cryptography;
using System.Text.Json;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

var repositoryRoot = FindRepositoryRoot();
var sourcePath = GetArgument("--source") ?? Path.Combine(repositoryRoot,
    "Tools", "StatsCollector", "Data", "Output", "MASTER_PLUS_SEEDED",
    "KR", "420", "16.17", "MIN900S", "matchups.json");
var destinationPath = Path.Combine(repositoryRoot, "Data", "Statistics", "master_plus.json");
var manifestPath = Path.Combine(repositoryRoot, "Data", "Statistics", "manifest.json");
var downloadUrl = GetArgument("--download-url");
var revisionText = GetArgument("--revision");
var revision = revisionText is null
    ? DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    : long.TryParse(revisionText, out var parsedRevision) && parsedRevision > 0
        ? parsedRevision
        : throw new ArgumentException("--revision must be a positive integer.");

Console.WriteLine("LoL Matchup statistics dataset publish");
Console.WriteLine($"Source: {sourcePath}");
Console.WriteLine($"Destination: {destinationPath}");

try
{
    var sourceBytes = await File.ReadAllBytesAsync(sourcePath);
    var dataset = JsonSerializer.Deserialize<StatisticsDataset>(sourceBytes);
    if (!StatisticsDatasetValidator.TryValidate(dataset, out var index, out var validationError))
    {
        throw new InvalidDataException(validationError);
    }

    var sha256 = Convert.ToHexString(SHA256.HashData(sourceBytes)).ToLowerInvariant();
    var manifest = new DatasetUpdateManifest
    {
        SchemaVersion = StatisticsDatasetValidator.SupportedSchemaVersion,
        DatasetId = DatasetUpdateOptions.DatasetId,
        DatasetRevision = revision,
        Patch = dataset!.Patch,
        TierScope = dataset.TierScope,
        Region = dataset.Region,
        QueueId = dataset.QueueId,
        MinimumGameDurationSeconds = dataset.MinimumGameDurationSeconds!.Value,
        AcceptedMatches = dataset.AcceptedMatches,
        FileSizeBytes = sourceBytes.LongLength,
        Sha256 = sha256,
        DownloadUrl = string.IsNullOrWhiteSpace(downloadUrl) ? null : downloadUrl
    };

    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
    await AtomicPublishAsync(
        destinationPath,
        sourceBytes,
        manifestPath,
        JsonSerializer.SerializeToUtf8Bytes(manifest,
            new JsonSerializerOptions { WriteIndented = true }));

    var publishedBytes = await File.ReadAllBytesAsync(destinationPath);
    var published = JsonSerializer.Deserialize<StatisticsDataset>(publishedBytes);
    if (!StatisticsDatasetValidator.TryValidate(published, out _, out _)
        || !SHA256.HashData(publishedBytes).SequenceEqual(SHA256.HashData(sourceBytes)))
    {
        throw new InvalidDataException("Published dataset verification failed.");
    }

    Console.WriteLine($"Patch: {dataset.Patch}");
    Console.WriteLine($"Accepted matches: {dataset.AcceptedMatches:N0}");
    Console.WriteLine($"Matchup keys: {index.Count:N0}");
    Console.WriteLine($"Minimum duration: {dataset.MinimumGameDurationSeconds} seconds");
    Console.WriteLine($"File size: {sourceBytes.LongLength:N0} bytes");
    Console.WriteLine($"SHA-256: {sha256}");
    Console.WriteLine($"Dataset revision: {revision}");
    Console.WriteLine("Publish success");
}
catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
    or JsonException or InvalidDataException or CryptographicException)
{
    Console.Error.WriteLine($"Publish failure: {exception.Message}");
    Environment.ExitCode = 1;
}

string? GetArgument(string name)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

static string FindRepositoryRoot()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "LoLMatchup.csproj")))
    {
        directory = directory.Parent;
    }
    return directory?.FullName
        ?? throw new DirectoryNotFoundException("LoLMatchup repository root was not found.");
}

static async Task AtomicPublishAsync(
    string datasetPath,
    byte[] datasetContent,
    string manifestPath,
    byte[] manifestContent)
{
    var temporaryDataset = datasetPath + ".tmp";
    var temporaryManifest = manifestPath + ".tmp";
    var datasetBackup = datasetPath + ".bak";
    var manifestBackup = manifestPath + ".bak";
    var datasetInstalled = false;
    try
    {
        await WriteCompleteFileAsync(temporaryDataset, datasetContent);
        await WriteCompleteFileAsync(temporaryManifest, manifestContent);
        if (File.Exists(datasetPath)) File.Copy(datasetPath, datasetBackup, overwrite: true);
        if (File.Exists(manifestPath)) File.Copy(manifestPath, manifestBackup, overwrite: true);
        File.Move(temporaryDataset, datasetPath, overwrite: true);
        datasetInstalled = true;
        File.Move(temporaryManifest, manifestPath, overwrite: true);
    }
    catch
    {
        if (datasetInstalled)
        {
            RestoreOrRemove(datasetPath, datasetBackup);
            RestoreOrRemove(manifestPath, manifestBackup);
        }
        throw;
    }
    finally
    {
        if (File.Exists(temporaryDataset)) File.Delete(temporaryDataset);
        if (File.Exists(temporaryManifest)) File.Delete(temporaryManifest);
    }
}

static async Task WriteCompleteFileAsync(string path, byte[] content)
{
    await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write,
        FileShare.None, 81920, useAsync: true);
    await stream.WriteAsync(content);
    await stream.FlushAsync();
}

static void RestoreOrRemove(string destination, string backup)
{
    if (File.Exists(backup)) File.Copy(backup, destination, overwrite: true);
    else if (File.Exists(destination)) File.Delete(destination);
}
