using System.IO;
using System.Text.Json;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services.Statistics;

namespace LoLMatchup.Services;

public sealed class StatisticsDataService
{
    private const string DatasetFileName = "master_plus.json";
    private readonly string _packagedDataFilePath;
    private readonly string _updatedDataFilePath;
    private readonly Lazy<Task<StatisticsDatasetLoadResult>> _loadResult;

    public StatisticsDataService(string? packagedDataFilePath = null, string? updatedDataFilePath = null)
    {
        _packagedDataFilePath = packagedDataFilePath ?? Path.Combine(
            AppContext.BaseDirectory, "Data", "Statistics", DatasetFileName);
        _updatedDataFilePath = updatedDataFilePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LoLMatchInfo", "Data", "Statistics", DatasetFileName);
        _loadResult = new Lazy<Task<StatisticsDatasetLoadResult>>(LoadCoreAsync);
    }

    public string UpdatedDataFilePath => _updatedDataFilePath;
    public Task<StatisticsDatasetLoadResult> LoadAsync() => _loadResult.Value;

    public async Task<DatasetUpdateManifest?> LoadCompanionManifestAsync(
        StatisticsDatasetSource source,
        CancellationToken cancellationToken = default)
    {
        var datasetPath = source == StatisticsDatasetSource.UpdatedLocal
            ? _updatedDataFilePath
            : _packagedDataFilePath;
        return await LoadManifestForDatasetAsync(datasetPath, cancellationToken);
    }

    public Task<DatasetUpdateManifest?> LoadInstalledUpdateManifestAsync(
        CancellationToken cancellationToken = default) =>
        LoadManifestForDatasetAsync(_updatedDataFilePath, cancellationToken);

    private static async Task<DatasetUpdateManifest?> LoadManifestForDatasetAsync(
        string datasetPath,
        CancellationToken cancellationToken)
    {
        var manifestPath = Path.Combine(Path.GetDirectoryName(datasetPath)!, "manifest.json");
        if (!File.Exists(manifestPath))
        {
            return null;
        }

        try
        {
            await using var stream = File.OpenRead(manifestPath);
            return await JsonSerializer.DeserializeAsync<DatasetUpdateManifest>(
                stream, cancellationToken: cancellationToken);
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException or JsonException)
        {
            return null;
        }
    }

    public async Task<StatisticsMatchupLookupResult?> FindMatchupAsync(
        string role, string firstChampionId, string secondChampionId)
    {
        if (!StatisticsDatasetValidator.TryCreateKey(
                role, firstChampionId, secondChampionId, out var key, out var wasReversed))
        {
            return null;
        }

        var loadResult = await LoadAsync();
        return loadResult.Status == StatisticsDatasetLoadStatus.Loaded
            && loadResult.MatchupIndex.TryGetValue(key, out var matchup)
                ? new StatisticsMatchupLookupResult(matchup, wasReversed)
                : null;
    }

    private async Task<StatisticsDatasetLoadResult> LoadCoreAsync()
    {
        var updated = await TryLoadAsync(_updatedDataFilePath, StatisticsDatasetSource.UpdatedLocal);
        return updated.Status == StatisticsDatasetLoadStatus.Loaded
            ? updated
            : await TryLoadAsync(_packagedDataFilePath, StatisticsDatasetSource.Packaged);
    }

    private static async Task<StatisticsDatasetLoadResult> TryLoadAsync(
        string path, StatisticsDatasetSource source)
    {
        if (!File.Exists(path))
        {
            return StatisticsDatasetLoadResult.NotAvailable;
        }

        try
        {
            await using var stream = File.OpenRead(path);
            var dataset = await JsonSerializer.DeserializeAsync<StatisticsDataset>(stream);
            return StatisticsDatasetValidator.TryValidate(dataset, out var index, out _)
                ? new StatisticsDatasetLoadResult(StatisticsDatasetLoadStatus.Loaded, dataset, index, source)
                : StatisticsDatasetLoadResult.Invalid;
        }
        catch (IOException)
        {
            return StatisticsDatasetLoadResult.NotAvailable;
        }
        catch (UnauthorizedAccessException)
        {
            return StatisticsDatasetLoadResult.NotAvailable;
        }
        catch (JsonException)
        {
            return StatisticsDatasetLoadResult.Invalid;
        }
    }
}

public enum StatisticsDatasetLoadStatus { Loaded, NotAvailable, Invalid }
public enum StatisticsDatasetSource { None, Packaged, UpdatedLocal }

public sealed record StatisticsDatasetLoadResult(
    StatisticsDatasetLoadStatus Status,
    StatisticsDataset? Dataset = null,
    IReadOnlyDictionary<StatisticsMatchupKey, StatisticsMatchupEntry>? Index = null,
    StatisticsDatasetSource Source = StatisticsDatasetSource.None)
{
    public static StatisticsDatasetLoadResult NotAvailable { get; } = new(StatisticsDatasetLoadStatus.NotAvailable);
    public static StatisticsDatasetLoadResult Invalid { get; } = new(StatisticsDatasetLoadStatus.Invalid);
    public IReadOnlyDictionary<StatisticsMatchupKey, StatisticsMatchupEntry> MatchupIndex =>
        Index ?? new Dictionary<StatisticsMatchupKey, StatisticsMatchupEntry>();
}
