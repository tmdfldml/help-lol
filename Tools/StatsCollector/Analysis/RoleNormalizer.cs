namespace StatsCollector.Analysis;

public static class RoleNormalizer
{
    public static IReadOnlyList<StandardRole> StandardRoles { get; } =
        [
            StandardRole.Top,
            StandardRole.Jungle,
            StandardRole.Mid,
            StandardRole.Adc,
            StandardRole.Support
        ];

    public static StandardRole Normalize(string? teamPosition)
    {
        return teamPosition?.Trim().ToUpperInvariant() switch
        {
            "TOP" => StandardRole.Top,
            "JUNGLE" => StandardRole.Jungle,
            "MIDDLE" => StandardRole.Mid,
            "BOTTOM" => StandardRole.Adc,
            "UTILITY" => StandardRole.Support,
            _ => StandardRole.Unknown
        };
    }

    public static string ToDisplayName(StandardRole role)
    {
        return role switch
        {
            StandardRole.Top => "TOP",
            StandardRole.Jungle => "JUNGLE",
            StandardRole.Mid => "MID",
            StandardRole.Adc => "ADC",
            StandardRole.Support => "SUPPORT",
            _ => "UNKNOWN"
        };
    }
}

public enum StandardRole
{
    Unknown,
    Top,
    Jungle,
    Mid,
    Adc,
    Support
}
