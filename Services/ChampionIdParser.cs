namespace LoLMatchup.Services;

public static class ChampionIdParser
{
    private const string RawNamePrefix = "game_character_displayname_";

    public static string? Parse(string? rawChampionName)
    {
        if (string.IsNullOrWhiteSpace(rawChampionName))
        {
            return null;
        }

        var rawName = rawChampionName.Trim();

        if (!rawName.StartsWith(RawNamePrefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var championId = rawName[RawNamePrefix.Length..];

        return championId.Length > 0 && championId.All(char.IsLetterOrDigit)
            ? championId
            : null;
    }
}
