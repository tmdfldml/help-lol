using System.Text.Json.Serialization;

namespace StatsCollector.Api.Dtos;

public sealed class MatchTimelineDto
{
    [JsonPropertyName("info")]
    public TimelineInfoDto Info { get; init; } = new();
}

public sealed class TimelineInfoDto
{
    [JsonPropertyName("frames")]
    public List<TimelineFrameDto> Frames { get; init; } = [];
}

public sealed class TimelineFrameDto
{
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; }

    [JsonPropertyName("events")]
    public List<TimelineEventDto> Events { get; init; } = [];

    [JsonPropertyName("participantFrames")]
    public Dictionary<string, TimelineParticipantFrameDto> ParticipantFrames
    {
        get;
        init;
    } = [];
}

public sealed class TimelineParticipantFrameDto
{
    [JsonPropertyName("participantId")]
    public int? ParticipantId { get; init; }

    [JsonPropertyName("level")]
    public int? Level { get; init; }
}

public sealed class TimelineEventDto
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; init; }

    [JsonPropertyName("participantId")]
    public int? ParticipantId { get; init; }

    [JsonPropertyName("level")]
    public int? Level { get; init; }

    [JsonPropertyName("killerId")]
    public int? KillerId { get; init; }

    [JsonPropertyName("victimId")]
    public int? VictimId { get; init; }

    [JsonPropertyName("assistingParticipantIds")]
    public List<int>? AssistingParticipantIds { get; init; }
}
