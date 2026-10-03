using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

// Developer-only Riot static data generator. No external statistics scraping or API key.
if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: ChampionSkillDataCollector <repository-root> <Data-Dragon-version>");
    return 1;
}
var root = Path.GetFullPath(args[0]);
var version = args[1];
if (!Regex.IsMatch(version, @"\A\d+\.\d+\.\d+\z")) throw new ArgumentException("Invalid version");
var output = Path.Combine(root, "Data", "ChampionSkills");
var cache = Path.Combine(root, "artifacts", "skill-source-cache", version);
Directory.CreateDirectory(cache);
Directory.CreateDirectory(Path.Combine(output, "Icons"));
using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
http.DefaultRequestHeaders.UserAgent.ParseAdd("LoLMatchup-SkillDataGenerator/1.0");
var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
var source = await Fetch($"data/ko_KR/championFull.json", "championFull.json");
using var full = JsonDocument.Parse(await File.ReadAllTextAsync(source));
if (full.RootElement.GetProperty("version").GetString() != version) throw new InvalidDataException("Version mismatch");
using var metadata = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(root, "Data", "Champions", "champions.ko.json")));
var ids = metadata.RootElement.GetProperty("champions").EnumerateArray().Select(c => c.GetProperty("id").GetString()!).ToArray();
if (ids.Distinct(StringComparer.OrdinalIgnoreCase).Count() != ids.Length) throw new InvalidDataException("Duplicate champion IDs");
var keys = new[] { "Q", "W", "E", "R" };
var documents = new Dictionary<string, object>();
var invalidCooldowns = new List<string>();
var missingCastTimers = new List<string>();
var missingRechargeTimers = new List<string>();
var audit = new List<object>();
foreach (var id in ids)
{
    if (!Regex.IsMatch(id, @"\A[A-Za-z][A-Za-z0-9]+\z")) throw new InvalidDataException("Invalid champion ID");
    var champion = full.RootElement.GetProperty("data").GetProperty(id);
    var spells = champion.GetProperty("spells").EnumerateArray().ToArray();
    if (spells.Length != 4) throw new InvalidDataException($"{id}: expected four spells");
    var converted = new List<object>();
    for (var i = 0; i < spells.Length; i++)
    {
        var spell = spells[i];
        var cooldown = spell.GetProperty("cooldown").EnumerateArray().Select(n => n.GetDouble()).ToArray();
        var maxRank = spell.GetProperty("maxrank").GetInt32();
        var valid = cooldown.Length == maxRank && maxRank is >= 1 and <= 6 && cooldown.All(c => double.IsFinite(c) && c > 0);
        var rankFiveToggle = version == "16.17.1" && id == "Urgot" && keys[i] == "W"
            && maxRank == 5 && cooldown.Length == 5 && cooldown.Take(4).All(c => double.IsFinite(c) && c > 0)
            && cooldown[4] == 0 && spell.GetProperty("tooltip").GetString()!.Contains("<toggle>", StringComparison.Ordinal);
        valid |= rankFiveToggle;
        var mechanic = CooldownClassifier.Classify(version, id, keys[i], spell, valid ? cooldown : null);
        if (mechanic.Type == "Unknown") invalidCooldowns.Add($"{id}.{keys[i]}");
        if (mechanic.Cooldowns is null) missingCastTimers.Add($"{id}.{keys[i]}");
        if (mechanic.Type == "Charge" && mechanic.RechargeTimes is null) missingRechargeTimers.Add($"{id}.{keys[i]}");
        audit.Add(new { champion = id, skill = keys[i], type = mechanic.Type,
            maxCharges = mechanic.MaxCharges, rechargeTimes = mechanic.RechargeTimes,
            castCooldowns = mechanic.Cooldowns, rawMaxAmmo = spell.GetProperty("maxammo").GetString(),
            rawCooldowns = cooldown, sourceEvidence = mechanic.Evidence,
            tooltip = spell.GetProperty("tooltip").GetString() });
        var image = spell.GetProperty("image");
        var sprite = image.GetProperty("sprite").GetString()!;
        if (!Regex.IsMatch(sprite, @"\Aspell\d+\.png\z")) throw new InvalidDataException("Unexpected sprite name");
        var spritePath = await Fetch($"img/sprite/{sprite}", sprite);
        var icon = $"{id}_{keys[i]}.png";
        using (var input = File.OpenRead(spritePath))
        {
            var decoder = BitmapDecoder.Create(input, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            var bitmap = new CroppedBitmap(decoder.Frames[0], new Int32Rect(
                image.GetProperty("x").GetInt32(), image.GetProperty("y").GetInt32(),
                image.GetProperty("w").GetInt32(), image.GetProperty("h").GetInt32()));
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using var target = File.Create(Path.Combine(output, "Icons", icon));
            encoder.Save(target);
        }
        var description = WebUtility.HtmlDecode(Regex.Replace(spell.GetProperty("description").GetString() ?? "", "<[^>]+>", " "));
        description = Regex.Replace(description, @"\s+", " ").Trim();
        // Preserve Riot's short description; never infer gameplay facts from templated tooltips.
        var entry = new Dictionary<string, object?> { ["key"] = keys[i], ["name"] = spell.GetProperty("name").GetString(),
            ["description"] = description, ["cooldownType"] = mechanic.Type,
            ["cooldowns"] = mechanic.Cooldowns, ["rechargeTimes"] = mechanic.RechargeTimes,
            ["maxCharges"] = mechanic.MaxCharges, ["mechanicNote"] = mechanic.Note,
            ["baseCooldowns"] = mechanic.Note is not null && mechanic.Type == "Charge" ? cooldown : null,
            ["sourceEvidence"] = mechanic.Evidence, ["icon"] = icon };
        if (rankFiveToggle) entry["cooldownNote"] = "5랭크: 토글 전환";
        converted.Add(entry);
    }
    documents.Add(id, new { schemaVersion = 2, championId = id, championName = champion.GetProperty("name").GetString(),
        patch = version, source = "RIOT_DATA_DRAGON", spells = converted });
}
// Write JSON only after every supported champion and icon has been checked.
foreach (var (id, data) in documents)
    await File.WriteAllTextAsync(Path.Combine(output, id + ".json"), JsonSerializer.Serialize(data, options));
await File.WriteAllTextAsync(Path.Combine(output, "manifest.json"), JsonSerializer.Serialize(new {
    schemaVersion = 2, patch = version, source = "RIOT_DATA_DRAGON", locale = "ko_KR",
    generatedAtUtc = DateTimeOffset.UtcNow, championCount = ids.Length, spellCount = ids.Length * 4,
    unavailableCooldowns = missingCastTimers,
    unknownCooldownMechanics = invalidCooldowns,
    unavailableRechargeTimes = missingRechargeTimers,
    sourceUrl = $"https://ddragon.leagueoflegends.com/cdn/{version}/data/ko_KR/championFull.json"
}, options));
await File.WriteAllTextAsync(Path.Combine(cache, "mechanic-audit.json"), JsonSerializer.Serialize(audit, options));
Console.WriteLine($"Generated {ids.Length} champions, {ids.Length * 4} icons; missing champions: 0; null cast timers: {missingCastTimers.Count}; unknown mechanics: {invalidCooldowns.Count}; unresolved recharge timers: {missingRechargeTimers.Count}");
return 0;

async Task<string> Fetch(string relative, string file)
{
    var path = Path.Combine(cache, file);
    if (File.Exists(path)) return path;
    await Task.Delay(400);
    using var response = await http.GetAsync($"https://ddragon.leagueoflegends.com/cdn/{version}/{relative}");
    response.EnsureSuccessStatusCode(); // Fail visibly; rerun resumes cached requests, without retry storms.
    var bytes = await response.Content.ReadAsByteArrayAsync();
    await File.WriteAllBytesAsync(path + ".partial", bytes);
    File.Move(path + ".partial", path, true);
    return path;
}
