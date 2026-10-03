using System.Diagnostics;
using System.IO;
using System.Text.Json;
using LoLMatchup.Models.Champions;
using LoLMatchup.Models.Navigation;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Services;
using LoLMatchup.Services.Diagnostics;
using LoLMatchup.Services.Logging;
using LoLMatchup.Services.Navigation;

internal static class SkillDataRegression
{
    public static async Task RunAsync()
    {
        var logger = new CounterLogger();
        var service = new ChampionSkillDataService(logger: logger);
        Require(logger.Count == 0, "Construction performs no reads");
        var metadata = await new ChampionMetadataService().GetAllChampionsAsync();
        var watch = Stopwatch.StartNew();
        var unavailable = 0;
        var charges = 0;
        foreach (var champion in metadata)
        {
            var data = await service.GetAsync(champion.ChampionId);
            Require(data is not null && data.Spells.Count == 4, "Every supported champion has four spells");
            Require(data!.Spells.Select(s => s!.Key).SequenceEqual(ChampionSkillDataService.SpellKeys), "Unique ordered QWER mapping");
            foreach (var spell in data!.Spells)
            {
                Require(spell is not null && service.GetIconPath(spell.Icon) is not null, "Every spell has a local icon");
                Require(!string.IsNullOrWhiteSpace(spell!.Name) && !string.IsNullOrWhiteSpace(spell.Description), "Localized spell text");
                if (spell.Cooldowns is null) unavailable++;
                Require(spell.CooldownText != "0초" && spell.RechargeText != "0초", "No bare zero-second display");
                Require(spell.CooldownType != SpellCooldownType.Standard || spell.Cooldowns is not null, "Standard timers valid");
                if (spell.IsCharge)
                {
                    charges++;
                    Require(spell.ChargeText.Contains("충전형 스킬"), "Charge mechanic label");
                    Require(spell.RechargeTimes is null || spell.RechargeTimes.All(t => double.IsFinite(t) && t > 0), "Recharge values valid");
                    Require(spell.Cooldowns is null || spell.Cooldowns.All(t => double.IsFinite(t) && t > 0), "Cast cooldown values valid");
                    Require(!string.IsNullOrWhiteSpace(spell.SourceEvidence), "Mechanic source evidence");
                }
            }
        }
        Require(metadata.Count == 173 && charges == 19, "Pinned 173 champions / 692 spells / 19 charge spells");
        Require((await service.GetAsync("Gangplank"))!.Spells[2]!.IsCharge, "Gangplank metadata charge classification");
        Require((await service.GetAsync("Karthus"))!.Spells[0]!.CooldownType == SpellCooldownType.Unknown, "Karthus remains unverified");
        Require((await service.GetAsync("Zeri"))!.Spells[0]!.CooldownType == SpellCooldownType.Unknown, "Zeri remains unverified");
        Require((await service.GetAsync("Singed"))!.Spells[0]!.CooldownText == "토글 스킬", "Singed toggle");
        Require((await service.GetAsync("Amumu"))!.Spells[1]!.CooldownText == "토글 스킬", "Second toggle");
        Require((await service.GetAsync("Shyvana"))!.Spells[3]!.CooldownText == "분노 기반", "Fury resource");
        Require((await service.GetAsync("Yunara"))!.Spells[0]!.CooldownText == "방출 중첩 기반", "Additional resource mechanic");
        Require((await service.GetAsync("Vi"))!.Spells[1]!.CooldownType == SpellCooldownType.Passive, "Passive-only slot");
        Require((await service.GetAsync("Aphelios"))!.Spells[2]!.CooldownType == SpellCooldownType.NotApplicable, "Non-ability slot");
        Require((await service.GetAsync("Syndra"))!.Spells[0]!.BaseCooldowns!.All(t => t == 7), "Conditional charge preserves base cooldown separately");
        Require((await service.GetAsync("Leblanc"))!.Spells[3]!.Cooldowns!.SequenceEqual(new double[] { 45,35,25 }), "Ambiguous maxammo does not discard Mimic cooldown");
        Require((await service.GetAsync("Warwick"))!.Spells[1]!.Cooldowns!.SequenceEqual(new double[] { 80,70,60,50,40 }), "Blood Hunt retains normal source cooldown");
        foreach (var spell in (await service.GetAsync("Rengar"))!.Spells.Take(3))
            Require(spell!.CooldownType == SpellCooldownType.Standard && spell.Cooldowns!.All(t => t == 0.25), "No charge mechanic invented from maxammo=1");
        var chargeFixture = new ChampionSkillData { SchemaVersion = 2, ChampionId = "Fixture", ChampionName = "fixture", Patch = "16.17.1",
            Spells = [new ChampionSpell { Key = "Q", Name = "Q", CooldownType = SpellCooldownType.Charge, Cooldowns = [1], RechargeTimes = [18,17,16,15,14], MaxCharges = 2 }] };
        var charge = ChampionSkillDataService.Validate(chargeFixture, "Fixture")!.Spells[0]!;
        Require(charge.CooldownText == "1초" && charge.RechargeText == "18 / 17 / 16 / 15 / 14초", "Independent cast and recharge timers");
        var badCharge = ChampionSkillDataService.Validate(chargeFixture with { Spells = [charge with { RechargeTimes = [0], Cooldowns = [-1], MaxCharges = -2 }] }, "Fixture")!.Spells[0]!;
        Require(badCharge.CooldownText == "정보 없음" && badCharge.RechargeText == "정보 없음" && badCharge.MaxCharges is null, "Invalid charge values isolated");
        Console.WriteLine($"Skills coverage PASS: {metadata.Count} champions, {metadata.Count * 4} icons, {unavailable} unavailable cooldowns; cold reads {watch.ElapsedMilliseconds} ms");
        var first = await service.GetAsync("Akali");
        var count = logger.Count;
        for (var i = 0; i < 100; i++) Require(ReferenceEquals(first, await service.GetAsync("Akali")), "Cache identity");
        Require(logger.Count == count, "Cache avoids repeated logging");
        Require(first!.Spells[3]!.Cooldowns!.Length == 3, "Ultimate rank count");
        var urgot = await service.GetAsync("Urgot");
        Require(urgot!.Spells[1]!.Cooldowns!.SequenceEqual(new double[] { 12, 9, 6, 3, 0 }), "Audited Urgot W preserves every source rank");
        Require(urgot.Spells[1]!.CooldownText.Contains("5랭크: 토글 전환"), "Zero rank explained as toggle");
        Require(await service.GetAsync("UnknownChampion") is null, "Unknown champion");
        Require(await service.GetAsync("../Akali") is null, "Path traversal rejected");
        Require(service.GetIconPath("https://example.com/a.png") is null, "Remote icons rejected");
        Require(await service.GetOrderAsync("Akali", "MID", first.Patch) is null, "Production empty order");

        var root = Path.Combine(Path.GetTempPath(), "LoLMatchup-SkillRegression-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await File.WriteAllTextAsync(Path.Combine(root, "Broken.json"), "{bad");
        var malformed = new ChampionSkillData { ChampionId = "Partial", ChampionName = "부분", Patch = "16.17.1",
            Spells = [new ChampionSpell { Key = "Q", Name = "Q", Cooldowns = [-1] },
                new ChampionSpell { Key = "W", Name = "W", Cooldowns = [5] },
                new ChampionSpell { Key = "W", Name = "duplicate", Cooldowns = [5] },
                new ChampionSpell { Key = "X", Name = "invalid" }] };
        await File.WriteAllTextAsync(Path.Combine(root, "Partial.json"), JsonSerializer.Serialize(malformed, jsonOptions));
        await File.WriteAllTextAsync(Path.Combine(root, "WrongId.json"), JsonSerializer.Serialize(first, jsonOptions));
        var malformedSpell = JsonSerializer.Serialize(first! with { ChampionId = "TypedBad" }, jsonOptions)
            .Replace("\"cooldowns\":[1.5,1.5,1.5,1.5,1.5]", "\"cooldowns\":\"bad\"");
        await File.WriteAllTextAsync(Path.Combine(root, "TypedBad.json"), malformedSpell);
        await File.WriteAllTextAsync(Path.Combine(root, "NoPatch.json"), JsonSerializer.Serialize(first! with { ChampionId = "NoPatch", Patch = "" }, jsonOptions));
        var orders = new List<ChampionSkillOrder>();
        foreach (var id in new[] { "Akali", "Gragas", "Pantheon" })
            foreach (var role in new[] { "TOP", "JUNGLE", "MID" })
                orders.Add(new() { ChampionId = id, Role = role, Patch = "16.17.1", Source = "SYNTHETIC_TEST",
                    CollectedAtUtc = DateTimeOffset.UtcNow, Priority = role == "TOP" ? ["Q", "E", "W"] : ["W", "Q", "E"] });
        await File.WriteAllTextAsync(Path.Combine(root, "orders.json"), JsonSerializer.Serialize(orders, jsonOptions));
        var synthetic = new ChampionSkillDataService(root);
        Require(await synthetic.GetAsync("Broken") is null && await synthetic.GetAsync("Missing") is null, "Malformed/missing safe");
        Require(await synthetic.GetAsync("WrongId") is null, "Wrong join key rejected");
        Require(await synthetic.GetAsync("NoPatch") is null, "Patch required");
        var typedBad = await synthetic.GetAsync("TypedBad");
        Require(typedBad!.Spells[0]!.Cooldowns is null && typedBad.Spells[1]!.Cooldowns is not null, "Malformed spell does not hide other spells");
        var partial = await synthetic.GetAsync("Partial");
        Require(partial!.Spells.Count == 4 && partial.Spells.All(s => s!.Cooldowns is null), "Invalid, duplicate, missing spells isolated");
        foreach (var id in new[] { "Akali", "Gragas", "Pantheon" })
        {
            Require((await synthetic.GetOrderAsync(id, "TOP", "16.17.1"))!.Priority[0] == "Q", "TOP isolated");
            Require((await synthetic.GetOrderAsync(id, "JUNGLE", "16.17.1"))!.Priority[0] == "W", "JUNGLE isolated");
            Require(await synthetic.GetOrderAsync(id, "SUPPORT", "16.17.1") is null, "No role fallback");
            Require(await synthetic.GetOrderAsync(id, "MID", "16.18.1") is null, "No patch fallback");
        }
        Require(AppDiagnosticsService.Compare("16.18.123", first.Patch) == PatchCompatibility.PatchMismatch, "Patch mismatch");
        var nav = new AppNavigationService();
        var context = new MatchupStatisticsPresentation(MatchupPresentationState.LoadedStatistics, "", Role: "TOP", OpponentChampionId: "Akali");
        nav.NavigateToChampionSkillDetail(context);
        Require(nav.CurrentPage == AppPage.ChampionSkillDetail && nav.SkillContext == context && nav.DetailContext is null, "Skill navigation");
        nav.Navigate(AppPage.InGame);
        Require(nav.SkillContext is null, "Back clears context");
        orders.Add(orders[0]);
        await File.WriteAllTextAsync(Path.Combine(root, "orders.json"), JsonSerializer.Serialize(orders, jsonOptions));
        Require(await new ChampionSkillDataService(root).GetOrderAsync("Akali", "TOP", "16.17.1") is null, "Duplicate order key rejected");
        await File.WriteAllTextAsync(Path.Combine(root, "orders.json"), "{malformed");
        Require(await new ChampionSkillDataService(root).GetOrderAsync("Akali", "TOP", "16.17.1") is null, "Malformed orders safe");
        Console.WriteLine("Skills safety, cache, role/patch isolation (3 multi-role champions), empty state, navigation: PASS");
    }

    private static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }
    private sealed class CounterLogger : IAppLogger
    {
        public int Count { get; private set; }
        public void Log(AppLogLevel level, AppLogEvent eventId, long? value = null, Exception? exception = null) => Count++;
    }
}
