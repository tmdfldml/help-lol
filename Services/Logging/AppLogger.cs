using System.IO;
using System.Text;
using LoLMatchup.Services.Diagnostics;

namespace LoLMatchup.Services.Logging;

/// <summary>Small, synchronous, bounded local logger. Every I/O path is best effort.</summary>
public sealed class AppLogger : IAppLogger
{
    private readonly object _gate = new();
    private readonly string _directory;
    private readonly AppLogLevel _minimum;
    private DateTime _lastCleanupDate;
    private const long MaximumDailyBytes = 4 * 1024 * 1024;
    public AppLogger(string? directory = null, AppLogLevel? minimum = null)
    {
        _directory = directory ?? Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "LoLMatchInfo", "Logs");
#if DEBUG
        _minimum = minimum ?? AppLogLevel.Debug;
#else
        _minimum = minimum ?? AppLogLevel.Info;
#endif
    }

    public void Log(AppLogLevel level, AppLogEvent eventId, long? value = null, Exception? exception = null)
    {
        if (level < _minimum) return;
        try
        {
            var text = $"{DateTimeOffset.UtcNow:O} [{level}] {eventId}"
                + (value.HasValue ? $" value={value.Value}" : "")
                + (exception is null ? "" : Environment.NewLine + SafeExceptionFormatter.Format(exception));
            WriteDaily(text);
        }
        catch { /* Diagnostics must not become a second application failure. */ }
    }

    public void LogSnapshot(AppDiagnosticsSnapshot snapshot)
    {
        try { WriteDaily($"{DateTimeOffset.UtcNow:O} [Info] Diagnostics\n{snapshot.ToLogText()}"); }
        catch { }
    }

    public string? WriteCrash(Exception exception, AppDiagnosticsSnapshot snapshot, AppLogEvent origin)
    {
        try
        {
            var now = DateTimeOffset.Now;
            var name = $"crash-{now:yyyyMMdd-HHmmss-fff}-{Environment.ProcessId}-{Guid.NewGuid():N}.log";
            var text = $"UTC: {now.UtcDateTime:O}\nLocal: {now:O}\nOrigin: {origin}\n"
                + snapshot.ToLogText() + "\n" + SafeExceptionFormatter.Format(exception);
            lock (_gate)
            {
                PrepareDirectory();
                File.WriteAllText(Path.Combine(_directory, name), text, new UTF8Encoding(false));
                CleanupCore();
            }
            return name;
        }
        catch { return null; }
    }

    public void Cleanup()
    {
        try { lock (_gate) { Directory.CreateDirectory(_directory); CleanupCore(); } }
        catch { }
    }

    private void WriteDaily(string text)
    {
        lock (_gate)
        {
            PrepareDirectory();
            var path = Path.Combine(_directory, $"app-{DateTime.Now:yyyy-MM-dd}.log");
            if (File.Exists(path) && new FileInfo(path).Length >= MaximumDailyBytes) return;
            // Shared read permits inspection. Other processes may briefly contend; failure is safe.
            using var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(stream, new UTF8Encoding(false));
            writer.WriteLine(text);
        }
    }

    private void PrepareDirectory()
    {
        Directory.CreateDirectory(_directory);
        if (_lastCleanupDate == DateTime.UtcNow.Date) return;
        CleanupCore();
        _lastCleanupDate = DateTime.UtcNow.Date;
    }

    private void CleanupCore()
    {
        var files = new DirectoryInfo(_directory).EnumerateFiles("*.log")
            .Where(f => f.Name.StartsWith("app-", StringComparison.Ordinal)
                || f.Name.StartsWith("crash-", StringComparison.Ordinal))
            .OrderByDescending(f => f.LastWriteTimeUtc).ToArray();
        for (var index = 0; index < files.Length; index++)
        {
            if (index < 100 && files[index].LastWriteTimeUtc >= DateTime.UtcNow.AddDays(-30)) continue;
            try { files[index].Delete(); } catch { }
        }
    }
}
