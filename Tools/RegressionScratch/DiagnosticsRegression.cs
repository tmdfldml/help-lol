using System.IO;
using LoLMatchup.Models.Statistics;
using LoLMatchup.Services;
using LoLMatchup.Services.Diagnostics;
using LoLMatchup.Services.Logging;

internal static class DiagnosticsRegression
{
    public static async Task RunAsync()
    {
        var root = Path.Combine(Path.GetTempPath(), "LoLMatchup-diagnostics-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            foreach (var (game, data, expected) in new (string?, string?, PatchCompatibility)[]
            {
                ("16.17.1", "16.17", PatchCompatibility.CurrentPatch),
                ("16.17", "16.17", PatchCompatibility.CurrentPatch),
                ("16.17.123.456", "16.17", PatchCompatibility.CurrentPatch),
                ("16.18.0", "16.17", PatchCompatibility.PatchMismatch),
                (null, "16.17", PatchCompatibility.Unknown),
                ("invalid", "16.17", PatchCompatibility.Unknown),
                ("16.17.bad", "16.17", PatchCompatibility.Unknown),
                ("16.17", "16", PatchCompatibility.Unknown),
                ("16.17.1.2.3", "16.17", PatchCompatibility.Unknown)
            }) Require(AppDiagnosticsService.Compare(game, data) == expected, "Patch comparison");
            Console.WriteLine("Patch normalization, mismatch, unknown and invalid versions: PASS");

            var diagnostics = new AppDiagnosticsService();
            var metadata = new StatisticsDataset { Patch = "16.17", Source = "SELF_RIOT", AcceptedMatches = 31 };
            diagnostics.SetDataset(new(StatisticsDatasetLoadStatus.Loaded, metadata,
                Source: StatisticsDatasetSource.Packaged), new() { DatasetRevision = 7 });
            var packaged = diagnostics.Snapshot;
            Require(packaged.DatasetRevision == 7 && packaged.LocationText == "기본 제공"
                && packaged.GeneratedText == "확인 불가", "Missing date/packaged presentation");
            diagnostics.SetDataset(new(StatisticsDatasetLoadStatus.Loaded, metadata,
                Source: StatisticsDatasetSource.UpdatedLocal), new() { DatasetRevision = 8 });
            diagnostics.SetCurrentGameVersion("16.18.0");
            Require(diagnostics.Snapshot.DatasetRevision == 8 && diagnostics.Snapshot.LocationText == "업데이트됨"
                && diagnostics.Snapshot.CompatibilityText.Contains("계속 사용할 수"), "Local/mismatch presentation");
            Require(packaged.DatasetRevision == 7 && packaged.CurrentGamePatch is null, "Snapshot immutable");
            diagnostics.SetCurrentGameVersion(null);
            Require(diagnostics.Snapshot.Compatibility == PatchCompatibility.Unknown, "Reset game patch");
            Console.WriteLine("Diagnostics sources, revisions, missing date, immutable snapshot and Korean warning: PASS");

            var directory = Path.Combine(root, "logs");
            var logger = new AppLogger(directory, AppLogLevel.Info);
            logger.Log(AppLogLevel.Debug, AppLogEvent.MatchupChanged);
            Parallel.For(0, 500, i => logger.Log(AppLogLevel.Info, AppLogEvent.CurrentRevision, i));
            var daily = Directory.GetFiles(directory, "app-*.log").Single();
            var lines = File.ReadAllLines(daily);
            Require(lines.Length == 500 && lines.All(l => l.Contains("[Info] CurrentRevision value=")), "Concurrent complete lines/debug suppressed");
            Require(lines.Select(l => l.Split("value=")[1]).Distinct().Count() == 500, "All writes retained");

            var forbidden = new[] { "RGAPI-", "RIOT_API_KEY", "Authorization:", "puuid", "riotId", "summonerName",
                "KR_123456789", "PrivatePerson#KR1", @"C:\Users\PrivatePerson", "secret-query" };
            var payload = string.Join(" ", forbidden) + new string('x', 100_000);
            Exception exception;
            try { throw new InvalidOperationException(payload, new ArgumentException(payload, new IOException(payload))); }
            catch (Exception caught) { exception = caught; }
            logger.Log(AppLogLevel.Critical, AppLogEvent.FatalDispatcher, exception: exception);
            var crash = logger.WriteCrash(exception, diagnostics.Snapshot, AppLogEvent.FatalDispatcher);
            Require(crash is not null, "Crash created");
            var crashText = File.ReadAllText(Path.Combine(directory, crash!));
            Require(crashText.Contains("Exception[0]") && crashText.Contains("Exception[1]")
                && crashText.Contains("Exception[2]") && crashText.Contains("UTC:")
                && crashText.Contains("Local:") && crashText.Contains(".NET:")
                && crashText.Contains("Stack trace") && crashText.Length < 20000, "Crash contents/bounded message");
            var nullStack = logger.WriteCrash(new Exception(payload), diagnostics.Snapshot, AppLogEvent.FatalStartup);
            Require(File.ReadAllText(Path.Combine(directory, nullStack!)).Contains("unavailable"), "Null stack safe");
            Exception deep = new Exception(payload);
            for (var i = 0; i < 100; i++) deep = new Exception(payload, deep);
            Require(SafeExceptionFormatter.Format(deep).Split("Exception[").Length == 13, "Exception depth bounded");
            var allLogs = string.Join("\n", Directory.GetFiles(directory).Select(File.ReadAllText));
            Require(forbidden.All(f => !allLogs.Contains(f, StringComparison.OrdinalIgnoreCase)), "Privacy scan");
            Console.WriteLine("UTF-8/concurrent 500 writes, verbosity, crash chain/null stack/depth/long message/privacy: PASS");

            var blocked = Path.Combine(root, "not-a-directory");
            File.WriteAllText(blocked, "file blocks directory creation");
            var failing = new AppLogger(blocked);
            failing.Log(AppLogLevel.Critical, AppLogEvent.FatalStartup, exception: exception);
            failing.LogSnapshot(diagnostics.Snapshot);
            Require(failing.WriteCrash(exception, diagnostics.Snapshot, AppLogEvent.FatalStartup) is null, "Write failure returns null");
            failing.Cleanup();
            Console.WriteLine("Logger path/write failure never throws; crash reports not saved: PASS");

            var old = Path.Combine(directory, "app-2000-01-01.log");
            File.WriteAllText(old, "old");
            File.SetLastWriteTimeUtc(old, DateTime.UtcNow.AddDays(-31));
            var unrelated = Path.Combine(directory, "keep.log");
            File.WriteAllText(unrelated, "untouched");
            File.SetLastWriteTimeUtc(unrelated, DateTime.UtcNow.AddDays(-40));
            logger.Cleanup();
            Require(!File.Exists(old) && File.Exists(unrelated) && File.Exists(daily), "Age cleanup ownership");
            for (var i = 0; i < 110; i++) File.WriteAllText(Path.Combine(directory, $"crash-fake-{i}.log"), "fake");
            logger.Cleanup();
            Require(Directory.GetFiles(directory, "*.log").Length == 101, "100 managed files plus untouched file");
            Console.WriteLine("30-day/100-file cleanup and unrelated file preservation: PASS");

            var invalid = Path.Combine(root, "invalid.json");
            File.WriteAllText(invalid, "malformed");
            var fallbackLogger = new RecordingLogger();
            var loader = new StatisticsDataService(updatedDataFilePath: invalid, logger: fallbackLogger);
            var loaded = await loader.LoadAsync();
            Require(loaded.Status == StatisticsDatasetLoadStatus.Loaded && loaded.Source == StatisticsDatasetSource.Packaged
                && fallbackLogger.Events.Contains(AppLogEvent.LocalDatasetFallback), "Local invalid fallback warns");
            var unavailable = await new StatisticsDataService(invalid, invalid).LoadAsync();
            Require(unavailable.Status != StatisticsDatasetLoadStatus.Loaded, "Both invalid detected for fatal startup");
            Console.WriteLine("Invalid local fallback warning and both-invalid startup prerequisite: PASS");
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static void Require(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException(name);
    }

    private sealed class RecordingLogger : IAppLogger
    {
        public List<AppLogEvent> Events { get; } = [];
        public void Log(AppLogLevel level, AppLogEvent eventId, long? value = null, Exception? exception = null) => Events.Add(eventId);
    }
}
