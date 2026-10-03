using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

// Classification is patch-scoped and based solely on the pinned Riot document.
internal static class CooldownClassifier
{
    internal sealed record Result(string Type, double[]? Cooldowns, double[]? RechargeTimes,
        int? MaxCharges, string? Note, string Evidence);

    public static Result Classify(string version, string id, string key, JsonElement spell, double[]? cooldown)
    {
        if (version != "16.17.1") throw new InvalidDataException("Review mechanic rules before changing the pinned patch.");
        var text = spell.GetProperty("tooltip").GetString() ?? "";
        var description = spell.GetProperty("description").GetString() ?? "";
        var ammoText = spell.GetProperty("maxammo").GetString();
        _ = int.TryParse(ammoText, out var ammo);
        var path = $"data.{id}.spells[{Array.IndexOf(new[] { "Q", "W", "E", "R" }, key)}]";
        Result Make(string type, double[]? cd, int? max = null, string? note = null, string evidence = "") =>
            new(type, cd, null, max, note, $"{path}: {evidence}");

        // maxammo is not a reliable universal charge flag. These source tooltips do not
        // corroborate stored casts/recharging (Karthus, Mimic, Blood Hunt, ferocity spells).
        // Do not invent a charge mechanic or discard an existing positive cooldown.
        if ((id, key) is ("Karthus", "Q") or ("Leblanc", "R") or ("Warwick", "W")
            || id == "Rengar" && key is "Q" or "W" or "E")
            return Make(cooldown is null ? "Unknown" : "Standard", cooldown,
                evidence: $"maxammo={ammoText}; tooltip has no confirmed stored-cast recharge; charge interpretation UNVERIFIED; positive cooldown[] preserved");
        if (id == "Urgot" && key == "W") return Make("Standard", cooldown,
            evidence: "cooldown=[12,9,6,3,0]; tooltip <toggle> at maximum rank");
        if (text.Contains("<toggle>", StringComparison.Ordinal))
            return Make("Toggle", null, evidence: "tooltip <toggle>; numeric switch delay is not presented as a standard cooldown");
        if (description.Contains("세 번째 스킬이 없습니다", StringComparison.Ordinal))
            return Make("NotApplicable", null, evidence: "description explicitly says no third skill");
        if (id == "Shyvana" && key == "R" && text.Contains("용의 분노"))
            return Make("ResourceBased", null, note: "분노 기반", evidence: "tooltip: generates/consumes Dragon Fury; cooldown all zero");
        if (id == "Ashe" && key == "Q" && text.Contains("중첩"))
            return Make("ResourceBased", null, note: "기본 공격 중첩 기반", evidence: "tooltip: basic attacks generate stacks required to activate; cooldown all zero");
        if (id == "Yunara" && key == "Q" && text.Contains("resource_max"))
            return Make("ResourceBased", null, note: "방출 중첩 기반", evidence: "tooltip: attacks generate Unleash stacks, activation consumes resource_max; cooldown all zero");
        if (cooldown is null && Regex.IsMatch(text, @"<(spellPassive|passive)>")
            && !Regex.IsMatch(text, @"<(spellActive|active|toggle)>") && !text.Contains("사용 시"))
            return Make("Passive", null, evidence: "passive-only tooltip, no active section; cooldown all zero");

        // Match storage/recharge vocabulary, not generic charging/channeling or recasts.
        var storage = Regex.IsMatch(text + description, @"회까지 충전|충전 횟수|재충전|ammo|recharge", RegexOptions.IgnoreCase);
        if (ammo > 1 || storage)
        {
            var conditional = text.Contains("upgrade1maxammo", StringComparison.Ordinal);
            // Rank-dependent unresolved maximum placeholders take precedence over the
            // scalar maxammo field. Do not claim its scalar is the maximum at every rank.
            var dynamicMaximum = Regex.IsMatch(text, @"\{\{\s*(maximumcharges|maxammotooltip|maxammo|maxkits|ammo_limit|upgrade1maxammo)\s*\}\}");
            var literal = Regex.Match(text, @"(?:이 스킬은 |충전 횟수는 )(\d+)회(?:까지 충전|이며)");
            int? max = literal.Success ? int.Parse(literal.Groups[1].Value) : null;
            return Make("Charge", conditional ? null : cooldown, max,
                conditional ? "진화 후 충전형 · 진화 후 시전 간격은 확인되지 않았습니다." : null,
                $"maxammo={ammoText}; " + (storage ? "tooltip storage/recharge metadata; " : "charge candidate from maxammo; ")
                + "recharge numeric values absent/unresolved; "
                + (conditional ? "base cooldown is not a confirmed evolved cast cooldown" : "cast cooldown from cooldown[] (positive values only)")
                + (dynamicMaximum ? "; maximum placeholder unresolved" : ""));
        }
        return Make(cooldown is null ? "Unknown" : "Standard", cooldown,
            evidence: cooldown is null ? "zero/invalid cooldown; mechanic or numeric timer unresolved; UNVERIFIED" : "cooldown[]");
    }
}
