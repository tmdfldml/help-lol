using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LoLMatchup.Models.Champions;
using LoLMatchup.Models.Presentation;
using LoLMatchup.Services;
using LoLMatchup.Services.Diagnostics;

namespace LoLMatchup.Views;

public partial class ChampionSkillDetailView : UserControl
{
    private int _request;
    public event EventHandler? BackRequested;
    public ChampionSkillDetailView() => InitializeComponent();

    public async Task SetContextAsync(MatchupStatisticsPresentation context, ChampionSkillDataService service, string? gamePatch)
    {
        var request = ++_request;
        ChampionTitle.Text = $"{context.OpponentChampionDisplayName} · {context.Role}";
        ChampionIcon.Source = context.OpponentChampionIcon;
        PatchText.Text = "스킬 데이터를 불러오는 중입니다.";
        WarningText.Text = "";
        StatusText.Text = "";
        SpellList.ItemsSource = null;
        OrderText.Text = "이 역할의 스킬 순서 통계를 아직 사용할 수 없습니다.";
        SequenceText.Text = SourceText.Text = "";
        DetailScroll.ScrollToTop();
        BackButton.Focus();
        var data = await service.GetAsync(context.OpponentChampionId);
        if (request != _request) return;
        if (data is null)
        {
            PatchText.Text = "스킬 데이터 없음";
            StatusText.Text = "스킬 정보를 사용할 수 없습니다.";
            return;
        }
        ChampionTitle.Text = $"{data.ChampionName} · {context.Role}";
        PatchText.Text = $"스킬 데이터 {data.Patch} · Riot Data Dragon";
        WarningText.Text = AppDiagnosticsService.Compare(gamePatch, data.Patch) == PatchCompatibility.PatchMismatch
            ? "현재 게임과 다른 패치의 스킬 정보입니다." : "기본 쿨타임 · 스킬 가속 미적용";
        SpellList.ItemsSource = data.Spells.Select(s => new SpellRow(s!, LoadIcon(service.GetIconPath(s!.Icon)))).ToArray();
        var order = await service.GetOrderAsync(data.ChampionId, context.Role, data.Patch);
        if (request != _request || order is null) return;
        OrderText.Text = string.Join(" → ", order.Priority);
        SequenceText.Text = order.LevelUpSequence is { Length: > 0 } sequence
            ? "대표 레벨업: " + string.Join(" ", sequence) : "";
        SourceText.Text = $"스킬 순서 통계 출처: {order.Source} · {order.Patch} · {order.Role}";
    }

    private static ImageSource? LoadIcon(string? path)
    {
        if (path is null) return null;
        try
        {
            using var stream = File.OpenRead(path);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException or NotSupportedException)
        { return null; }
    }

    private void Back_Click(object sender, RoutedEventArgs e) => BackRequested?.Invoke(this, EventArgs.Empty);
    private sealed record SpellRow(ChampionSpell Spell, ImageSource? Icon);
}
