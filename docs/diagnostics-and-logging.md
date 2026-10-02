# Local diagnostics (Stage 10-A)

The WPF application owns one `AppLogger` and one `AppDiagnosticsService` in `App`.
Constructor injection connects the loader, polling, optional champion metadata and
Settings updater. No DI package, telemetry, upload, API key or server is introduced.
Console publisher keeps a no-op logger unless explicitly supplied, preserving its
separation from the desktop application. Statistical policies and schemas are unchanged.

## Files and retention

- `%LocalAppData%\LoLMatchInfo\Logs\app-yyyy-MM-dd.log`: UTF-8, UTC event timestamps.
- `crash-yyyyMMdd-HHmmss-fff-PID-unique.log`: separate fatal diagnostic, UTC and local time.
- Settings and installed statistics retain their existing LocalAppData paths.
- Debug accepts Debug and higher; Release accepts Info and higher.
- A process-local lock protects complete writes. A daily file stops accepting records
  at 4 MiB (plus at most one bounded record). Fatal files are still attempted.
- At first write each UTC day, and after a crash file, retain at most 100 managed
  files and 30 days. Cleanup affects only `app-*.log` and `crash-*.log` in Logs.
- All log creation/write/cleanup failures are swallowed at the logging boundary.
  Fatal UI reports when crash-file creation failed, instead of claiming it was saved.
- Logging is deliberately synchronous and low volume, without an unbounded queue.
  Cross-process contention can lose an individual diagnostic record; records are not
  interleaved. This is a best-effort diagnostic facility, not an audit trail.

## Exception and privacy policy

Expected network unavailability, updater validation/network failures and optional
metadata/icon failures remain recoverable. Invalid installed data warns and falls
back to packaged data. If neither dataset can load, MainWindow invokes fatal startup.
Startup resource errors, Dispatcher unhandled errors, AppDomain unhandled errors and
unobserved Task errors use the fatal path: Critical event, crash file, Korean dialog,
exit code 1. The Dispatcher is marked handled only to allow this presentation and exit.
An interlocked guard prevents recursive crash handling. Fatal background paths do not
block on the UI dispatcher; they show the dialog on the reporting thread and exit.
After the dialog, WPF Shutdown is attempted on the UI thread, with process exit as the
last resort. Stack overflow, process kill and power loss cannot be guaranteed to log.

The logging API accepts fixed enum events and numeric values, not arbitrary strings.
No raw URLs, HTTP headers, environment dump, player JSON, player or match identities
cross this boundary. Dataset patch is parsed numerically and source is allowlisted.
Crash diagnostics contain runtime app/build/.NET/OS/architecture, active dataset
revision/patch/source/location/generated/count and patch compatibility.

`SafeExceptionFormatter` deliberately **does not read exception.Message, Data,
ToString or source-file paths**. It records type, HResult, a safe category message,
method-only stack frames and inner exception structure. This loses original message
detail but avoids unknown user identifiers embedded in framework/custom exceptions.
There are at most 12 exceptions and 48 frames each; null stacks are allowed. Raw
exception messages never reach user UI. Synthetic privacy tests use dummy markers,
not real credentials. Actual local logs must not be copied into the repository.

## Patches and freshness

Dataset metadata is the patch/generated/source/count authority. The active companion
manifest supplies revision; installed-but-not-restarted revision stays updater state.
The immutable common snapshot supplies Settings and crash/startup diagnostics.
App version comes from the diagnostics assembly's assembly version (three components).

Existing Live Client calls are `playerlist` and `activeplayername`; neither supplies
game version. The [official Riot Live Client documentation](https://developer.riotgames.com/docs/lol#game-client-api_live-client-data-api)
and [official sample](https://static.developer.riotgames.com/docs/lol/liveclientdata_sample.json)
were inspected. Sample `gameData` has gameMode/gameTime/mapName/mapNumber/mapTerrain,
not a game-version field. Stage 10-A therefore supplies **Unknown in production**.
There are no new game-version requests and no inference from dates, Data Dragon or
manifest timestamps. A future verified runtime source can call `SetCurrentGameVersion`.

`16.17`, `16.17.1`, `16.17.123.456` normalize to `16.17`. Invalid/missing versions
produce Unknown. States are CurrentPatch, PatchMismatch, Unknown; no guessed
PreviousPatch or age-based stale threshold. Missing generated time is `확인 불가`.
Settings displays a nonmodal mismatch explanation that explicitly permits continued
use of current statistics. Debug fixture can set a synthetic next-minor patch and reset
to Unknown without modifying any data file.

## Lifecycle and verification

Polling logs Waiting/GameDetected only on transitions and matchup changes only when
the signature changes. Ordinary no-game connection failures are not errors. Updater
wrappers log check/current/remote revision, download and all outcome categories;
installed success records restart required. Existing validation and install code stays
in place, including size/SHA/metadata/security/no-downgrade/conflict/rollback policies.

Production async entrypoints are awaited WPF event handlers. There is no production
fire-and-forget task. Top-level unexpected failures reach the global fatal handler.
Polling cancellation terminates the timer/HTTP work at window close; Settings cancels
and disposes its updater and guards post-await UI updates after close. Cancellation
sources remain valid for pending continuations and are subsequently collected.

Debug crash and synthetic patch handlers are in DeveloperTestWindow, whose code and
BAML are removed in Release by the existing project rules. The unobserved Task fixture
intentionally drops a faulted Task and requests GC; finalizer timing remains a runtime
property. Use an isolated Debug run: both crash buttons terminate the app.

Run `dotnet run --project Tools/RegressionScratch/RegressionScratch.csproj` after
building both app configurations. This covers synthetic diagnostics, real shared
statistics regression, and PE/BAML/fixture method exclusion. The existing publisher
`--self-test` covers updater security/rollback. `--verify-remote <production manifest>`
does GET-only verification and temporary validation, never production writes.
See `regression-test-report.md` for observed runtime results and remaining manual checks.

## Stage 10-A changed files

- `App.xaml.cs`
- `MainWindow.xaml.cs`
- `Services/Logging/IAppLogger.cs` (new)
- `Services/Logging/AppLogger.cs` (new)
- `Services/Logging/SafeExceptionFormatter.cs` (new)
- `Services/Diagnostics/AppDiagnosticsService.cs` (new)
- `Services/StatisticsDataService.cs`
- `Services/ChampionMetadataService.cs`
- `Services/LiveStatisticsPollingService.cs`
- `Services/Statistics/DatasetUpdateService.cs`
- `Views/SettingsWindow.xaml`
- `Views/SettingsWindow.xaml.cs`
- `Views/DeveloperTestWindow.xaml`
- `Views/DeveloperTestWindow.xaml.cs`
- `Tools/PublishStatisticsDataset/PublishStatisticsDataset.csproj`
- `Tools/RegressionScratch/Program.cs`
- `Tools/RegressionScratch/DiagnosticsRegression.cs` (new)
- `PROJECT_STATUS.md`
- `docs/regression-test-report.md`
- `docs/diagnostics-and-logging.md` (new)