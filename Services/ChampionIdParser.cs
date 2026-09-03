using System.Diagnostics;

namespace LoLMatchup.Services;

public static class ChampionIdParser
{
    private const string DisplayNamePrefix = "game_character_displayname_";
    private const string CharacterNamePrefix = "Character_";
    private const string CharacterNameSuffix = "_Name";

    static ChampionIdParser()
    {
#if DEBUG
        Debug.Assert(ParseCore("game_character_displayname_Aatrox") == "Aatrox");
        Debug.Assert(ParseCore("Character_Aatrox_Name") == "Aatrox");
        Debug.Assert(ParseCore("Character_LeeSin_Name") == "LeeSin");
        Debug.Assert(ParseCore(null) is null);
        Debug.Assert(ParseCore(string.Empty) is null);
        Debug.Assert(ParseCore("malformed") is null);
        Debug.WriteLine("ChampionIdParser format validation: True");
#endif
    }

    public static string? Parse(string? rawChampionName) =>
        ParseCore(rawChampionName);

    private static string? ParseCore(string? rawChampionName)
    {
        if (string.IsNullOrWhiteSpace(rawChampionName))
        {
            return null;
        }

        var rawName = rawChampionName.Trim();

        string championId;

        if (rawName.StartsWith(
                DisplayNamePrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            championId = rawName[DisplayNamePrefix.Length..];
        }
        else if (rawName.StartsWith(
                     CharacterNamePrefix,
                     StringComparison.OrdinalIgnoreCase)
                 && rawName.EndsWith(
                     CharacterNameSuffix,
                     StringComparison.OrdinalIgnoreCase))
        {
            championId = rawName[
                CharacterNamePrefix.Length..
                ^CharacterNameSuffix.Length];
        }
        else
        {
            return null;
        }

        return championId.Length > 0 && championId.All(char.IsLetterOrDigit)
            ? championId
            : null;
    }
}
