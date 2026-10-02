using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using LoLMatchup.Models.Statistics;

namespace LoLMatchup.Services.Diagnostics;

public enum PatchCompatibility { Unknown, CurrentPatch, PatchMismatch }

public sealed record AppDiagnosticsSnapshot(
    string AppVersion, string BuildConfiguration, string DotNetVersion, string OsVersion,
    string Architecture, string? DatasetPatch = null, long? DatasetRevision = null,
    DateTimeOffset? DatasetGeneratedAtUtc = null, string DatasetSource = "Unknown",
    StatisticsDatasetSource DatasetLocation = StatisticsDatasetSource.None,
    int? DatasetAcceptedMatches = null, string? CurrentGamePatch = null,
    PatchCompatibility Compatibility = PatchCompatibility.Unknown)
{
    public string CompatibilityText => Compatibility switch
    {
        PatchCompatibility.CurrentPatch => "현재 게임 패치와 일치",
        PatchCompatibility.PatchMismatch => "게임 패치와 다른 통계입니다. 기존 통계는 계속 사용할 수 있습니다.",
        _ => "현재 연결 정보로는 게임 패치를 확인할 수 없습니다. 통계는 계속 확인할 수 있습니다."
    };
    public string LocationText => DatasetLocation switch
    {
        StatisticsDatasetSource.Packaged => "기본 제공",
        StatisticsDatasetSource.UpdatedLocal => "업데이트됨",
        _ => "—"
    };
    public string RevisionText => DatasetRevision is > 0 ? DatasetRevision.Value.ToString(CultureInfo.InvariantCulture) : "기본 버전";
    public string GeneratedText => DatasetGeneratedAtUtc?.ToLocalTime().ToString("yyyy-MM-dd HH:mm") ?? "확인 불가";

    internal string ToLogText() => $"App: {AppVersion}; Build: {BuildConfiguration}; .NET: {DotNetVersion}; OS: {OsVersion}; Architecture: {Architecture}\n"
        + $"Dataset patch: {DatasetPatch ?? "Unknown"}; revision: {DatasetRevision}; source: {DatasetSource}; location: {DatasetLocation}; generated UTC: {DatasetGeneratedAtUtc:O}; matches: {DatasetAcceptedMatches}\n"
        + $"Game patch: {CurrentGamePatch ?? "Unknown"}; compatibility: {Compatibility}";
}

/// <summary>One immutable active-session snapshot; installed-but-not-restarted updates do not change it.</summary>
public sealed class AppDiagnosticsService
{
    private AppDiagnosticsSnapshot _snapshot = CreateInitial();
    public AppDiagnosticsSnapshot Snapshot => Volatile.Read(ref _snapshot);
    public event EventHandler? Changed;

    public void SetDataset(StatisticsDatasetLoadResult result, DatasetUpdateManifest? manifest)
    {
        var data = result.Dataset;
        var snapshot = Snapshot with
        {
            DatasetPatch = NormalizePatch(data?.Patch),
            DatasetRevision = manifest?.DatasetRevision is > 0 ? manifest.DatasetRevision : null,
            DatasetGeneratedAtUtc = data?.GeneratedAtUtc is { } date && date != default ? date : null,
            DatasetSource = data?.Source is "SELF_RIOT" or "OPGG" or "LOLPS" or "OTHER" ? data.Source : "Unknown",
            DatasetLocation = result.Source,
            DatasetAcceptedMatches = data?.AcceptedMatches
        };
        Publish(snapshot);
    }

    // Existing playerlist/activeplayername responses do not contain a reliable game version.
    // Production supplies null. A future verified runtime source can use this boundary, without Web API calls.
    public void SetCurrentGameVersion(string? version) => Publish(Snapshot with { CurrentGamePatch = NormalizePatch(version) });

    private void Publish(AppDiagnosticsSnapshot snapshot)
    {
        snapshot = snapshot with { Compatibility = Compare(snapshot.CurrentGamePatch, snapshot.DatasetPatch) };
        if (snapshot == Snapshot) return;
        Volatile.Write(ref _snapshot, snapshot);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public static PatchCompatibility Compare(string? game, string? dataset)
    {
        var left = NormalizePatch(game);
        var right = NormalizePatch(dataset);
        return left is null || right is null ? PatchCompatibility.Unknown
            : left == right ? PatchCompatibility.CurrentPatch : PatchCompatibility.PatchMismatch;
    }

    public static string? NormalizePatch(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 64) return null;
        var parts = value.Split('.');
        if (parts.Length is < 2 or > 4) return null;
        var numbers = new int[parts.Length];
        for (var i = 0; i < parts.Length; i++)
            if (!int.TryParse(parts[i], NumberStyles.None, CultureInfo.InvariantCulture, out numbers[i])) return null;
        return numbers[0] <= 0 ? null : $"{numbers[0]}.{numbers[1]}";
    }

    private static AppDiagnosticsSnapshot CreateInitial()
    {
        var version = typeof(AppDiagnosticsService).Assembly.GetName().Version;
        return new(version?.ToString(3) ?? "Unknown",
#if DEBUG
            "Debug",
#else
            "Release",
#endif
            Environment.Version.ToString(), Environment.OSVersion.Version.ToString(),
            RuntimeInformation.ProcessArchitecture.ToString());
    }
}
