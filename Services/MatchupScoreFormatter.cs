namespace LoLMatchup.Services;

public static class MatchupScoreFormatter
{
    public static string ToDebugText(int? score)
    {
        return score switch
        {
            2 => "매우 유리",
            1 => "유리",
            0 => "반반",
            -1 => "불리",
            -2 => "매우 불리",
            _ => "데이터 없음"
        };
    }
}
