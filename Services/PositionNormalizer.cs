namespace LoLMatchup.Services;

public static class PositionNormalizer
{
    public const string Top = "TOP";
    public const string Jungle = "JUNGLE";
    public const string Mid = "MID";
    public const string Adc = "ADC";
    public const string Support = "SUPPORT";
    public const string Unknown = "UNKNOWN";

    public static IReadOnlyList<string> StandardPositions { get; } =
        [Top, Jungle, Mid, Adc, Support];

    public static string Normalize(string? position)
    {
        return position?.Trim().ToUpperInvariant() switch
        {
            "TOP" => Top,
            "JUNGLE" => Jungle,
            "MIDDLE" => Mid,
            "BOTTOM" => Adc,
            "UTILITY" => Support,
            _ => Unknown
        };
    }
}
