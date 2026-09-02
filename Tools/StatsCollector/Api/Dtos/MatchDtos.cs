using System.Text.Json.Serialization;

namespace StatsCollector.Api.Dtos;

public sealed class MatchDto
{
    [JsonPropertyName("metadata")]
    public MatchMetadataDto Metadata { get; init; } = new();

    [JsonPropertyName("info")]
    public MatchInfoDto Info { get; init; } = new();
}

public sealed class MatchMetadataDto
{
    [JsonPropertyName("matchId")]
    public string MatchId { get; init; } = string.Empty;
}

public sealed class MatchInfoDto
{
    [JsonPropertyName("queueId")]
    public int QueueId { get; init; }

    [JsonPropertyName("gameVersion")]
    public string GameVersion { get; init; } = string.Empty;

    [JsonPropertyName("gameDuration")]
    public long GameDuration { get; init; }

    [JsonPropertyName("participants")]
    public List<MatchParticipantDto> Participants { get; init; } = [];
}

public sealed class MatchParticipantDto
{
    [JsonPropertyName("participantId")]
    public int ParticipantId { get; init; }

    [JsonPropertyName("puuid")]
    public string Puuid { get; init; } = string.Empty;

    [JsonPropertyName("teamId")]
    public int TeamId { get; init; }

    [JsonPropertyName("championId")]
    public int ChampionId { get; init; }

    [JsonPropertyName("championName")]
    public string ChampionName { get; init; } = string.Empty;

    [JsonPropertyName("teamPosition")]
    public string TeamPosition { get; init; } = string.Empty;

    [JsonPropertyName("win")]
    public bool Win { get; init; }
}
