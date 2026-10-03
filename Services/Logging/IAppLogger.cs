namespace LoLMatchup.Services.Logging;

public enum AppLogLevel { Debug, Info, Warning, Error, Critical }

// Only fixed event names and numeric values cross the log boundary. No raw payloads/URLs.
public enum AppLogEvent
{
    Startup, Shutdown, FatalDispatcher, FatalDomain, FatalTask, FatalStartup,
    LocalDatasetFallback, OptionalMetadataFallback, OptionalIconFallback,
    ManifestCheckStarted, CurrentRevision, ManifestRevision, UpToDate, NoDowngrade,
    UpdateAvailable, DownloadStarted, ValidationFailed, InstallSucceeded,
    RestartRequired, UpdateFailed, UpdateCancelled, RollbackFailed, CleanupFailed,
    Waiting, GameDetected, MatchupChanged, LiveClientUnavailable,
    SkillDataLoaded, SkillDataMissing, SkillDataMalformed, SkillOrderUnavailable
}

/// <summary>Implementations must never throw; callers supply fixed events, not user data.</summary>
public interface IAppLogger
{
    void Log(AppLogLevel level, AppLogEvent eventId, long? value = null, Exception? exception = null);
}

public sealed class NullAppLogger : IAppLogger
{
    public static NullAppLogger Instance { get; } = new();
    private NullAppLogger() { }
    public void Log(AppLogLevel level, AppLogEvent eventId, long? value = null, Exception? exception = null) { }
}
