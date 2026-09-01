using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LoLMatchup.Services;

public sealed class MatchupDataService
{
    private static readonly string DataFilePath = Path.Combine(
        AppContext.BaseDirectory,
        "Data",
        "matchups.json");

    private readonly Lazy<Task<MatchupData?>> _data;

    public MatchupDataService()
    {
        _data = new Lazy<Task<MatchupData?>>(LoadFromFileAsync);
    }

    public Task<MatchupData?> LoadAsync()
    {
        return _data.Value;
    }

    public async Task<ChampionMatchupData?> FindMatchupAsync(
        string role,
        string firstChampionId,
        string secondChampionId)
    {
        if (string.IsNullOrWhiteSpace(role)
            || string.IsNullOrWhiteSpace(firstChampionId)
            || string.IsNullOrWhiteSpace(secondChampionId))
        {
            return null;
        }

        var normalizedRole = role.Trim().ToUpperInvariant();

        if (!PositionNormalizer.StandardPositions.Contains(normalizedRole))
        {
            return null;
        }

        var firstId = firstChampionId.Trim();
        var secondId = secondChampionId.Trim();

        var championA = firstId;
        var championB = secondId;

        if (StringComparer.OrdinalIgnoreCase.Compare(championA, championB) > 0)
        {
            (championA, championB) = (championB, championA);
        }

        var data = await LoadAsync();

        return data?.Matchups.FirstOrDefault(matchup =>
            matchup is not null
            && string.Equals(matchup.Role, normalizedRole, StringComparison.OrdinalIgnoreCase)
            && string.Equals(
                matchup.ChampionA,
                championA,
                StringComparison.OrdinalIgnoreCase)
            && string.Equals(
                matchup.ChampionB,
                championB,
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task<int?> GetMatchupScoreAsync(
        string role,
        string firstChampionId,
        string secondChampionId)
    {
        var matchup = await FindMatchupAsync(
            role,
            firstChampionId,
            secondChampionId);

        if (matchup is null || matchup.ScoreForA is < -2 or > 2)
        {
            return null;
        }

        var isStoredDirection = string.Equals(
            firstChampionId.Trim(),
            matchup.ChampionA,
            StringComparison.OrdinalIgnoreCase);

        return isStoredDirection ? matchup.ScoreForA : -matchup.ScoreForA;
    }

    private static async Task<MatchupData?> LoadFromFileAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(DataFilePath);
            var data = JsonSerializer.Deserialize<MatchupData>(json);

            return data?.Matchups is not null ? data : null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}

public sealed class MatchupData
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; init; }

    [JsonPropertyName("dataType")]
    public string DataType { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("matchups")]
    public List<ChampionMatchupData> Matchups { get; init; } = [];
}

public sealed class ChampionMatchupData
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;

    [JsonPropertyName("championA")]
    public string ChampionA { get; init; } = string.Empty;

    [JsonPropertyName("championB")]
    public string ChampionB { get; init; } = string.Empty;

    [JsonPropertyName("scoreForA")]
    public int ScoreForA { get; init; }
}
