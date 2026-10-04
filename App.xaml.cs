using System.Windows;
using LoLMatchup.Services.Diagnostics;
using LoLMatchup.Services.Logging;

namespace LoLMatchup;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public AppLogger Logger { get; } = new();
    public AppDiagnosticsService Diagnostics { get; } = new();
    private int _fatalStarted;

    public App()
    {
        // Registered before generated InitializeComponent loads essential resources.
        DispatcherUnhandledException += (_, e) =>
        {
            e.Handled = true; // Present diagnostics and terminate, never resume.
            HandleFatal(e.Exception, AppLogEvent.FatalDispatcher);
        };
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            HandleFatal(e.ExceptionObject as Exception ?? new InvalidOperationException(), AppLogEvent.FatalDomain);
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            e.SetObserved();
            HandleFatal(e.Exception, AppLogEvent.FatalTask);
        };
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Logger.Log(AppLogLevel.Info, AppLogEvent.Startup);
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (Volatile.Read(ref _fatalStarted) == 0) Logger.Log(AppLogLevel.Info, AppLogEvent.Shutdown);
        base.OnExit(e);
    }

    public void HandleFatal(Exception exception, AppLogEvent origin)
    {
        if (Interlocked.Exchange(ref _fatalStarted, 1) != 0) Environment.Exit(1);
        try
        {
            Logger.Log(AppLogLevel.Critical, origin, exception: exception);
            var crashFile = Logger.WriteCrash(exception, Diagnostics.Snapshot, origin);
            var diagnosticMessage = crashFile is null
                ? "진단 로그를 저장하지 못했습니다. 저장 공간과 폴더 권한을 확인해 주세요."
                : "진단 로그가 %LocalAppData%\\LoLMatchInfo\\Logs에 저장되었습니다.\n참조: " + crashFile;
            MessageBox.Show("앱에서 예상하지 못한 오류가 발생하여 종료합니다.\n"
                + "앱을 다시 실행해 주세요. 문제가 반복되면 진단 로그를 확인해 주세요.\n\n"
                + diagnosticMessage, "LoL Match Info 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            if (Dispatcher.CheckAccess()) Shutdown(1);
        }
        catch { /* Last-resort exit also covers failure in WPF, logging or shutdown. */ }
        finally { Environment.Exit(1); }
    }
}

