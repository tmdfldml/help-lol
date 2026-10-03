using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Services;
using LoLMatchup.Views;

internal static class SkillUiRegression
{
    // In-process WPF control tests; no test controls are compiled into the application.
    public static void Run(bool shell = false)
    {
        var app = shell ? new LoLMatchup.App() : new Application();
        app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/LoLMatchInfo;component/Views/StatisticsViewResources.xaml", UriKind.Relative) });
        app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/LoLMatchInfo;component/Views/AppShellResources.xaml", UriKind.Relative) });
        app.Startup += async (_, _) =>
        {
            try
            {
                LoLMatchup.MainWindow? window = null;
                if (shell)
                {
                    window = new LoLMatchup.MainWindow();
                    typeof(LoLMatchup.MainWindow).GetField("_hasStartedPolling", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(window, true);
                    // This layout test neither polls a real game nor persists window settings.
                    var closing = typeof(LoLMatchup.MainWindow).GetMethod("MainWindow_Closing", BindingFlags.Instance | BindingFlags.NonPublic)!;
                    window.Closing -= closing.CreateDelegate<CancelEventHandler>(window);
                    window.Show();
                    ((FrameworkElement)window.FindName("HomeView")).Visibility = Visibility.Collapsed;
                }
                var view = window is null ? new ChampionSkillDetailView() : (ChampionSkillDetailView)window.FindName("ChampionSkillDetailView");
                view.Visibility = Visibility.Visible;
                var service = new ChampionSkillDataService();
                foreach (var (id, role) in new[] { ("Akali", "TOP"), ("Vi", "JUNGLE"), ("Syndra", "MID"),
                    ("Amumu", "JUNGLE"), ("Ashe", "ADC"), ("Caitlyn", "ADC"), ("Singed", "TOP"),
                    ("Shyvana", "JUNGLE"), ("Aphelios", "ADC"), ("Urgot", "TOP") })
                {
                    var display = await new ChampionMetadataService().GetDisplayInfoAsync(id);
                    var context = new MatchupStatisticsPresentation(MatchupPresentationState.LoadedStatistics, "", Role: role,
                        OpponentChampionId: id, OpponentChampionDisplayName: display.KoreanDisplayName, OpponentChampionIcon: display.IconSource);
                    await view.SetContextAsync(context, service, "16.18.1");
                    Require(((ItemsControl)view.FindName("SpellList")).Items.Count == 4, "Four QWER rows");
                    Require(((TextBlock)view.FindName("WarningText")).Text.Contains("다른 패치"), "Mismatch is visible");
                    Require(((TextBlock)view.FindName("OrderText")).Text.Contains("아직"), "Order empty state");
                    foreach (var size in new[] { new Size(380, 630), new Size(400, 690), new Size(420,760), new Size(440,820), new Size(720, 290) })
                    {
                        if (window is null) { view.Measure(size); view.Arrange(new Rect(size)); view.UpdateLayout(); }
                        else
                        {
                            window.MinWidth = 0; window.MinHeight = 0;
                            window.Width = size.Width == 720 ? 760 : size.Width < 420 ? size.Width + 40 : size.Width;
                            window.Height = size.Height == 290 ? 420 : size.Height < 760 ? size.Height + 130 : size.Height;
                            window.UpdateLayout();
                            await System.Windows.Threading.Dispatcher.Yield(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                            window.UpdateLayout();
                        }
                        var texts = Descendants(view).OfType<TextBlock>().Where(t => VisibleInTree(t)).ToArray();
                        Require(texts.Count(t => t.Text == "재사용 대기시간") == 4, "Four cooldown headings");
                        var chargeCount = (await service.GetAsync(id))!.Spells.Count(s => s!.IsCharge);
                        Require(texts.Count(t => t.Text == "충전 시간") == chargeCount, "Separate recharge headings");
                        Require(texts.All(t => t.ActualWidth <= size.Width && t.ActualHeight + t.Margin.Top + t.Margin.Bottom + 1 >= t.DesiredSize.Height), "Wrapped text has space, no vertical clipping");
                        var scroller = (ScrollViewer)view.FindName("DetailScroll");
                        Require(scroller.ExtentWidth <= scroller.ViewportWidth + 1, "No horizontal overflow");
                        scroller.ScrollToTop(); view.UpdateLayout();
                        var bitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
                        bitmap.Render(view);
                        var output = Path.Combine(AppContext.BaseDirectory, shell ? "skill-shell-ui-renders" : "skill-ui-renders");
                        Directory.CreateDirectory(output);
                        if (window is not null)
                        {
                            var shellBitmap = new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth), (int)Math.Ceiling(window.ActualHeight), 96,96,PixelFormats.Pbgra32);
                            shellBitmap.Render(window);
                            var shellEncoder = new PngBitmapEncoder(); shellEncoder.Frames.Add(BitmapFrame.Create(shellBitmap));
                            using var shellStream = File.Create(Path.Combine(output, $"{id}-shell-{window.Width}x{window.Height}.png"));
                            shellEncoder.Save(shellStream);
                        }
                        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        using var stream = File.Create(Path.Combine(output, $"{id}-{size.Width}x{size.Height}.png"));
                        encoder.Save(stream);
                        foreach (var card in Descendants(view).OfType<Border>().Where(b => b.DataContext?.GetType().Name == "SpellRow"))
                        {
                            card.BringIntoView(); view.UpdateLayout();
                            var cardBitmap = new RenderTargetBitmap((int)Math.Ceiling(card.ActualWidth), (int)Math.Ceiling(card.ActualHeight), 96,96,PixelFormats.Pbgra32);
                            cardBitmap.Render(card);
                            var cardEncoder = new PngBitmapEncoder(); cardEncoder.Frames.Add(BitmapFrame.Create(cardBitmap));
                            var key = Descendants(card).OfType<TextBlock>().First(t => new[] { "Q", "W", "E", "R" }.Contains(t.Text)).Text;
                            using var cardStream = File.Create(Path.Combine(output, $"{id}-{key}-{size.Width}x{size.Height}.png"));
                            cardEncoder.Save(cardStream);
                        }
                    }
                }
                var missing = new MatchupStatisticsPresentation(MatchupPresentationState.LoadedStatistics, "", Role: "TOP", OpponentChampionId: "UnknownChampion");
                await view.SetContextAsync(missing, service, null);
                Require(((ItemsControl)view.FindName("SpellList")).Items.Count == 0, "Missing clears previous champion");
                Require(((TextBlock)view.FindName("StatusText")).Text.Contains("사용할 수 없습니다"), "Unknown champion UI");
                var root = Path.Combine(Path.GetTempPath(), "LoLMatchup-SkillUi-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(root);
                await File.WriteAllTextAsync(Path.Combine(root, "UnknownChampion.json"), "{malformed");
                await view.SetContextAsync(missing, new ChampionSkillDataService(root), null);
                Require(((TextBlock)view.FindName("StatusText")).Text.Contains("사용할 수 없습니다"), "Malformed UI");
                File.Copy(Path.Combine(AppContext.BaseDirectory, "Data", "ChampionSkills", "Akali.json"), Path.Combine(root, "Akali.json"));
                var akali = missing with { OpponentChampionId = "Akali" };
                await view.SetContextAsync(akali, new ChampionSkillDataService(root), null);
                Require(((ItemsControl)view.FindName("SpellList")).Items.Count == 4, "Missing icons keep spells readable");
                await File.WriteAllTextAsync(Path.Combine(root, "orders.json"), """
                    [{"championId":"Akali","role":"TOP","patch":"16.17.1","source":"SYNTHETIC_TEST",
                    "collectedAtUtc":"2026-10-02T00:00:00Z","priority":["Q","E","W"],"levelUpSequence":["Q","E","W"]}]
                    """);
                await view.SetContextAsync(akali, new ChampionSkillDataService(root), null);
                Require(((TextBlock)view.FindName("OrderText")).Text == "Q → E → W", "Priority UI");
                Require(((TextBlock)view.FindName("SequenceText")).Text.Contains("Q E W"), "Sequence UI");
                Require(((TextBlock)view.FindName("SourceText")).Text.Contains("SYNTHETIC_TEST"), "Attribution UI");
                await view.SetContextAsync(akali with { Role = "MID" }, new ChampionSkillDataService(root), null);
                Require(((TextBlock)view.FindName("OrderText")).Text.Contains("아직"), "No cross-role UI fallback");
                Console.WriteLine($"WPF skill {(shell ? "application shell" : "control")} smoke ({new LoLMatchup.Services.Diagnostics.AppDiagnosticsService().Snapshot.BuildConfiguration}) PASS: 10 champions / 5 sizes, separate charge rows, no clipping/overflow, Urgot, patch warning, empty order, malformed, stale content cleared");
                app.Shutdown(0);
            }
            catch (Exception e) { Console.Error.WriteLine(e); app.Shutdown(1); Environment.ExitCode = 1; }
        };
        Environment.ExitCode = app.Run();
    }
    private static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }
    private static IEnumerable<DependencyObject> Descendants(DependencyObject node)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(node); i++)
        {
            var child = VisualTreeHelper.GetChild(node, i);
            yield return child;
            foreach (var descendant in Descendants(child)) yield return descendant;
        }
    }
    private static bool VisibleInTree(DependencyObject node)
    {
        for (var current = node; current is not null; current = VisualTreeHelper.GetParent(current))
            if (current is UIElement element && element.Visibility != Visibility.Visible) return false;
        return true;
    }
}
