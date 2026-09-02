namespace StatsCollector.Analysis;

public static class PatchVersionParser
{
    public static bool TryParseGameVersion(
        string? gameVersion,
        out PatchVersion patchVersion)
    {
        patchVersion = default;

        if (string.IsNullOrWhiteSpace(gameVersion))
        {
            return false;
        }

        var components = gameVersion.Split('.');

        return components.Length >= 2
            && TryCreate(components[0], components[1], out patchVersion);
    }

    public static bool TryParseTargetPatch(
        string? targetPatch,
        out PatchVersion patchVersion)
    {
        patchVersion = default;

        if (string.IsNullOrWhiteSpace(targetPatch))
        {
            return false;
        }

        var components = targetPatch.Split('.');

        return components.Length == 2
            && TryCreate(components[0], components[1], out patchVersion);
    }

    private static bool TryCreate(
        string majorComponent,
        string minorComponent,
        out PatchVersion patchVersion)
    {
        patchVersion = default;

        if (!int.TryParse(majorComponent, out var major)
            || !int.TryParse(minorComponent, out var minor)
            || major < 0
            || minor < 0)
        {
            return false;
        }

        patchVersion = new PatchVersion(major, minor);
        return true;
    }
}

public readonly record struct PatchVersion(int Major, int Minor)
{
    public override string ToString() => $"{Major}.{Minor}";
}
