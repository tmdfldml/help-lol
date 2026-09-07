namespace LoLMatchup.Models.Settings;

public enum AppLayoutMode
{
    Portrait,
    Landscape
}

public sealed record AppSettings
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;
    public bool AlwaysOnTop { get; init; }
    public bool RememberWindowPosition { get; init; } = true;
    public bool RememberWindowSize { get; init; } = true;
    public double? LastWindowLeft { get; init; }
    public double? LastWindowTop { get; init; }
    public double? LastWindowWidth { get; init; }
    public double? LastWindowHeight { get; init; }
    public AppLayoutMode LastLayoutMode { get; init; } = AppLayoutMode.Portrait;
    public double PortraitWidth { get; init; } = 440;
    public double PortraitHeight { get; init; } = 820;
    public double LandscapeWidth { get; init; } = 900;
    public double LandscapeHeight { get; init; } = 600;
}
