namespace LoLMatchup.Models.Presentation;

public enum CompactDirectionLevel
{
    Unavailable,
    Neutral,
    Ally,
    Enemy
}

public sealed record CompactDirectionIndicatorPresentation(
    string Text,
    CompactDirectionLevel Level);
