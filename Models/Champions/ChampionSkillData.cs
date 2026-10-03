using System.Globalization;
using System.Text.Json.Serialization;

namespace LoLMatchup.Models.Champions;

public sealed record ChampionSkillData
{
    public int SchemaVersion { get; init; } = 1;
    public string ChampionId { get; init; } = "";
    public string ChampionName { get; init; } = "";
    public string Patch { get; init; } = "";
    public string Source { get; init; } = "RIOT_DATA_DRAGON";
    public List<ChampionSpell?> Spells { get; init; } = [];
}

public sealed record ChampionSpell
{
    public string Key { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    /// <summary>Time until another cast is allowed; never the charge recovery timer.</summary>
    public double[]? Cooldowns { get; init; }
    public SpellCooldownType CooldownType { get; init; } = SpellCooldownType.Standard;
    /// <summary>Time to recover one stored cast. Null when Riot supplies only an unresolved template.</summary>
    public double[]? RechargeTimes { get; init; }
    public int? MaxCharges { get; init; }
    public double[]? BaseCooldowns { get; init; }
    public string? MechanicNote { get; init; }
    public string? SourceEvidence { get; init; }
    public string? CooldownNote { get; init; }
    public string? Icon { get; init; }
    public bool IsCharge => CooldownType == SpellCooldownType.Charge;
    public string ChargeText => !IsCharge ? "" : "충전형 스킬"
        + (MaxCharges is > 0 ? $" · 최대 {MaxCharges}회" : "")
        + (string.IsNullOrEmpty(MechanicNote) ? "" : "\n" + MechanicNote);
    public string RechargeText => FormatTimes(RechargeTimes);
    public bool HasBaseCooldown => IsCharge && BaseCooldowns is { Length: > 0 };
    public string BaseCooldownText => FormatTimes(BaseCooldowns);
    public string CooldownText => CooldownType switch
    {
        SpellCooldownType.Toggle => "토글 스킬",
        SpellCooldownType.ResourceBased => MechanicNote ?? "자원 기반",
        SpellCooldownType.Passive => "해당 없음 · 지속 효과",
        SpellCooldownType.NotApplicable => "해당 없음",
        SpellCooldownType.Unknown => "정보 없음",
        _ => FormatTimes(Cooldowns) + (string.IsNullOrEmpty(CooldownNote) ? "" : "\n" + CooldownNote)
    };
    private static string FormatTimes(double[]? values) => values is { Length: > 0 }
        ? string.Join(" / ", values.Select(x => x.ToString("0.##", CultureInfo.InvariantCulture))) + "초"
        : "정보 없음";
}

[JsonConverter(typeof(JsonStringEnumConverter<SpellCooldownType>))]
public enum SpellCooldownType { Standard, Charge, Toggle, ResourceBased, Passive, NotApplicable, Unknown }

public sealed record ChampionSkillOrder
{
    public string ChampionId { get; init; } = "";
    public string Role { get; init; } = "";
    public string Patch { get; init; } = "";
    public string Source { get; init; } = "";
    public DateTimeOffset CollectedAtUtc { get; init; }
    public string[] Priority { get; init; } = [];
    public string[]? LevelUpSequence { get; init; }
    public int? SampleSize { get; init; }
    public double? PickRate { get; init; }
    public double? WinRate { get; init; }
}
