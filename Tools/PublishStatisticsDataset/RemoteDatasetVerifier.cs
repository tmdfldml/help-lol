using LoLMatchup.Services;
using LoLMatchup.Services.Statistics;

internal static class RemoteDatasetVerifier
{
    public static async Task<int> RunAsync(string manifestUrl)
    {
        if (!Uri.TryCreate(manifestUrl, UriKind.Absolute, out var manifestUri)
            || manifestUri.Scheme != Uri.UriSchemeHttps)
        {
            Console.Error.WriteLine("Remote verification requires an absolute HTTPS manifest URL.");
            return 1;
        }

        var temporaryRoot = Path.Combine(
            Path.GetTempPath(),
            $"lolmatchup-remote-verification-{Guid.NewGuid():N}");
        var datasetPath = Path.Combine(temporaryRoot, "master_plus.json");
        Directory.CreateDirectory(temporaryRoot);

        try
        {
            using var updater = new DatasetUpdateService(
                new DatasetUpdateOptions { ManifestUri = manifestUri },
                datasetPath);
            var check = await updater.CheckAsync(currentRevision: 0);
            if (check.Status != DatasetUpdateStatus.UpdateAvailable
                || check.Manifest is null)
            {
                Console.Error.WriteLine(
                    $"Remote manifest verification failed: {check.Status} - {check.Message}");
                return 1;
            }

            var install = await updater.InstallAsync(check.Manifest);
            if (install.Status != DatasetUpdateStatus.Installed)
            {
                Console.Error.WriteLine(
                    $"Remote dataset verification failed: {install.Status} - {install.Message}");
                return 1;
            }

            var dataService = new StatisticsDataService(
                packagedDataFilePath: datasetPath,
                updatedDataFilePath: Path.Combine(temporaryRoot, "unused", "master_plus.json"));
            var loaded = await dataService.LoadAsync();
            if (loaded.Status != StatisticsDatasetLoadStatus.Loaded
                || loaded.Dataset is null)
            {
                Console.Error.WriteLine("Installed remote dataset could not be loaded.");
                return 1;
            }

            var manifest = check.Manifest;
            var datasetUri = new Uri(manifestUri, manifest.DownloadUrl);
            Console.WriteLine("Remote statistics verification");
            Console.WriteLine($"Manifest URL: {manifestUri}");
            Console.WriteLine($"Dataset URL: {datasetUri}");
            Console.WriteLine($"Revision: {manifest.DatasetRevision}");
            Console.WriteLine($"Patch: {manifest.Patch}");
            Console.WriteLine($"Accepted matches: {manifest.AcceptedMatches:N0}");
            Console.WriteLine($"Observations: {manifest.Observations:N0}");
            Console.WriteLine($"Matchup keys: {manifest.MatchupKeys:N0}");
            Console.WriteLine($"File size: {manifest.FileSizeBytes:N0} bytes");
            Console.WriteLine($"SHA-256: {manifest.Sha256.ToLowerInvariant()}");
            Console.WriteLine("Manifest GET and validation: PASS");
            Console.WriteLine("Dataset GET, size and SHA-256: PASS");
            Console.WriteLine("Shared dataset validation: PASS");
            Console.WriteLine("Manifest and dataset metadata: PASS");
            return 0;
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException or HttpRequestException
            or InvalidDataException or ArgumentException)
        {
            Console.Error.WriteLine($"Remote verification failed: {exception.Message}");
            return 1;
        }
        finally
        {
            try
            {
                Directory.Delete(temporaryRoot, recursive: true);
            }
            catch (Exception exception) when (exception is IOException
                or UnauthorizedAccessException)
            {
                Console.Error.WriteLine(
                    $"Remote verification temporary cleanup warning: {exception.Message}");
            }
        }
    }
}
