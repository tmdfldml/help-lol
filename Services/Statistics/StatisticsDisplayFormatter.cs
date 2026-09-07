using System.Globalization;

namespace LoLMatchup.Services.Statistics;

public static class StatisticsDisplayFormatter
{
    private static readonly CultureInfo DisplayCulture =
        CultureInfo.GetCultureInfo("ko-KR");

    public static string Percentage(double value) =>
        value.ToString("P1", DisplayCulture);

    public static string EventsPerMatch(double value) =>
        $"{value.ToString("F2", DisplayCulture)} / 경기";

    public static string CompactEventsPerMatch(double value) =>
        $"{value.ToString("F2", DisplayCulture)}회/경기";

    public static string Decimal2(double value) =>
        value.ToString("F2", DisplayCulture);
}
