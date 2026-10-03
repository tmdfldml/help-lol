using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using LoLMatchup.Models.Champions;
using LoLMatchup.Services.Diagnostics;
using LoLMatchup.Services.Logging;

namespace LoLMatchup.Services;

/// <summary>Optional local content. No network or player identity crosses this boundary.</summary>
public sealed class ChampionSkillDataService
{
    public static IReadOnlyList<string> SpellKeys { get; } = ["Q", "W", "E", "R"];
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly string _root;
    private readonly IAppLogger _logger;
    private readonly ConcurrentDictionary<string, Lazy<Task<ChampionSkillData?>>> _cache = new(StringComparer.Ordinal);
    private readonly Lazy<Task<Dictionary<(string, string, string), ChampionSkillOrder>>> _orders;

    public ChampionSkillDataService(string? root = null, IAppLogger? logger = null)
    {
        _root = root ?? Path.Combine(AppContext.BaseDirectory, "Data", "ChampionSkills");
        _logger = logger ?? NullAppLogger.Instance;
        _orders = new(LoadOrdersAsync);
    }

    public Task<ChampionSkillData?> GetAsync(string championId) => IsId(championId)
        ? _cache.GetOrAdd(championId, id => new(() => LoadAsync(id))).Value
        : Task.FromResult<ChampionSkillData?>(null);

    public async Task<ChampionSkillOrder?> GetOrderAsync(string id, string role, string patch)
    {
        var orders = await _orders.Value;
        return orders.GetValueOrDefault((id, role, patch));
    }

    public string? GetIconPath(string? file) => file is not null
        && Regex.IsMatch(file, @"\A[A-Za-z0-9_.-]+\.png\z")
        && File.Exists(Path.Combine(_root, "Icons", file)) ? Path.Combine(_root, "Icons", file) : null;

    public static bool IsId(string? id) => id is not null && Regex.IsMatch(id, @"\A[A-Za-z][A-Za-z0-9]{0,63}\z");

    public static ChampionSkillData? Validate(ChampionSkillData? data, string expectedId)
    {
        if (data is null || data.SchemaVersion is not (1 or 2) || data.ChampionId != expectedId
            || !IsId(data.ChampionId) || string.IsNullOrWhiteSpace(data.ChampionName)
            || AppDiagnosticsService.NormalizePatch(data.Patch) is null || data.Source != "RIOT_DATA_DRAGON") return null;
        var spells = data.Spells ?? [];
        return data with { Spells = SpellKeys.Select(key =>
        {
            var matches = spells.Where(s => s?.Key == key).ToArray();
            var s = matches.Length == 1 ? matches[0] : null;
            if (s is null || string.IsNullOrWhiteSpace(s.Name))
                return new ChampionSpell { Key = key, Name = "스킬 정보 없음", Description = "스킬 정보를 사용할 수 없습니다." };
            var validCooldown = s.Cooldowns is { Length: >= 1 and <= 6 }
                && s.Cooldowns.All(c => double.IsFinite(c) && c > 0);
            // Audited 16.17.1 Urgot W: retain ranks 1–4; rank 5 becomes a toggle.
            var rankFiveToggle = data.ChampionId == "Urgot" && data.Patch == "16.17.1" && key == "W"
                && s.Cooldowns is { Length: 5 } cd && cd.Take(4).All(c => double.IsFinite(c) && c > 0) && cd[4] == 0;
            validCooldown |= rankFiveToggle;
            var type = Enum.IsDefined(s.CooldownType) ? s.CooldownType : SpellCooldownType.Unknown;
            var recharge = s.RechargeTimes is { Length: >= 1 and <= 6 }
                && s.RechargeTimes.All(c => double.IsFinite(c) && c > 0) ? s.RechargeTimes : null;
            if (type == SpellCooldownType.Standard && !validCooldown) type = SpellCooldownType.Unknown;
            return s with { CooldownType = type,
                Cooldowns = validCooldown && type is (SpellCooldownType.Standard or SpellCooldownType.Charge) ? s.Cooldowns : null,
                RechargeTimes = type == SpellCooldownType.Charge ? recharge : null,
                BaseCooldowns = type == SpellCooldownType.Charge && s.BaseCooldowns is { Length: >= 1 and <= 6 }
                    && s.BaseCooldowns.All(c => double.IsFinite(c) && c > 0) ? s.BaseCooldowns : null,
                MaxCharges = type == SpellCooldownType.Charge && s.MaxCharges is > 0 and <= 100 ? s.MaxCharges : null,
                CooldownNote = rankFiveToggle ? "5랭크: 토글 전환" : null,
                Description = string.IsNullOrWhiteSpace(s.Description) ? "설명 정보 없음" : s.Description };
        }).Cast<ChampionSpell?>().ToList() };
    }

    private async Task<ChampionSkillData?> LoadAsync(string id)
    {
        try
        {
            await using var stream = File.OpenRead(Path.Combine(_root, id + ".json"));
            // Deserialize each spell independently so a corrupt cooldown cannot hide the other spells.
            using var json = await JsonDocument.ParseAsync(stream);
            var node = json.RootElement;
            var header = new ChampionSkillData
            {
                ChampionId = node.GetProperty("championId").GetString() ?? "",
                ChampionName = node.GetProperty("championName").GetString() ?? "",
                Patch = node.GetProperty("patch").GetString() ?? "",
                Source = node.GetProperty("source").GetString() ?? "",
                SchemaVersion = node.GetProperty("schemaVersion").GetInt32()
            };
            var spells = new List<ChampionSpell?>();
            if (node.TryGetProperty("spells", out var array) && array.ValueKind == JsonValueKind.Array)
                foreach (var item in array.EnumerateArray())
                    try { spells.Add(item.Deserialize<ChampionSpell>(JsonOptions)); }
                    catch (JsonException) { spells.Add(null); }
            var result = Validate(header with { Spells = spells }, id);
            _logger.Log(result is null ? AppLogLevel.Warning : AppLogLevel.Info,
                result is null ? AppLogEvent.SkillDataMalformed : AppLogEvent.SkillDataLoaded);
            return result;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidOperationException or KeyNotFoundException or FormatException)
        {
            _logger.Log(AppLogLevel.Warning, ex is FileNotFoundException or DirectoryNotFoundException
                ? AppLogEvent.SkillDataMissing : AppLogEvent.SkillDataMalformed);
            return null;
        }
    }

    private async Task<Dictionary<(string, string, string), ChampionSkillOrder>> LoadOrdersAsync()
    {
        var result = new Dictionary<(string, string, string), ChampionSkillOrder>();
        var duplicates = new HashSet<(string, string, string)>();
        try
        {
            await using var stream = File.OpenRead(Path.Combine(_root, "orders.json"));
            using var json = await JsonDocument.ParseAsync(stream);
            foreach (var node in json.RootElement.EnumerateArray())
            {
                ChampionSkillOrder? order;
                try { order = node.Deserialize<ChampionSkillOrder>(JsonOptions); }
                catch (JsonException) { continue; }
                if (order is null || !IsId(order.ChampionId) || !PositionNormalizer.StandardPositions.Contains(order.Role)
                    || AppDiagnosticsService.NormalizePatch(order.Patch) is null || string.IsNullOrWhiteSpace(order.Source)
                    || order.CollectedAtUtc == default || order.Priority is not { Length: 3 }
                    || !order.Priority.Order().SequenceEqual(new[] { "E", "Q", "W" })
                    || order.LevelUpSequence is { } seq && (seq.Length is < 1 or > 18 || seq.Any(k => !SpellKeys.Contains(k)))
                    || order.SampleSize is < 0 || !ValidRatio(order.PickRate) || !ValidRatio(order.WinRate)) continue;
                var key = (order.ChampionId, order.Role, order.Patch);
                if (!result.TryAdd(key, order)) duplicates.Add(key);
            }
            foreach (var key in duplicates) result.Remove(key);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidOperationException)
        { _logger.Log(AppLogLevel.Warning, AppLogEvent.SkillOrderUnavailable); }
        return result;
    }

    private static bool ValidRatio(double? value) => value is null || double.IsFinite(value.Value) && value is >= 0 and <= 1;
}
