using LoLMatchup.Models.Development;

namespace LoLMatchup.Services.Development;

public sealed class DeveloperTestModeService
{
    private const string ActivePlayerId = "TestActive#DEV";
    private int _version;

    public bool IsActive => CurrentScenario is not null;
    public DeveloperTestScenario? CurrentScenario { get; private set; }
    public int Version => _version;

    public event EventHandler? StateChanged;

    public void StartScenario(DeveloperTestScenario scenario)
    {
        CurrentScenario = scenario;
        _version++;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        CurrentScenario = null;
        _version++;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public static DeveloperTestScenario CreateNormal5v5() => Create(
        "정상 5v5",
        ["Aatrox", "LeeSin", "Ahri", "Kaisa", "Nautilus"],
        ["Akali", "Vi", "Syndra", "Jhin", "Thresh"]);

    public static DeveloperTestScenario CreateTop1v1() => Create(
        "TOP 1v1",
        ["Aatrox", null, null, null, null],
        ["Akali", null, null, null, null]);

    public static DeveloperTestScenario CreatePartialRoles() => Create(
        "일부 Role 누락",
        ["Aatrox", "LeeSin", null, "Kaisa", "Nautilus"],
        ["Akali", "Vi", null, "Jhin", "Thresh"]);

    public static DeveloperTestScenario CreateNotFound() => Create(
        "통계 없음",
        ["UnknownChampion123", null, null, null, null],
        ["UnknownChampion456", null, null, null, null]);

    public static DeveloperTestScenario CreateCustom(
        IReadOnlyList<string?> allies,
        IReadOnlyList<string?> enemies) => Create("직접 구성", allies, enemies);

    private static DeveloperTestScenario Create(
        string name,
        IReadOnlyList<string?> allies,
        IReadOnlyList<string?> enemies)
    {
        var players = new List<LiveClientPlayer>(10);
        var roles = PositionNormalizer.StandardPositions;
        var activeRoleIndex = Enumerable.Range(0, roles.Count)
            .FirstOrDefault(index => !string.IsNullOrWhiteSpace(allies[index]), -1);

        for (var index = 0; index < roles.Count; index++)
        {
            AddPlayer(
                players,
                allies[index],
                roles[index],
                "ORDER",
                index,
                index == activeRoleIndex);
            AddPlayer(players, enemies[index], roles[index], "CHAOS", index, false);
        }

        if (activeRoleIndex < 0)
        {
            players.Add(new LiveClientPlayer
            {
                RiotId = ActivePlayerId,
                Team = "ORDER"
            });
        }

        return new DeveloperTestScenario(name, ActivePlayerId, players);
    }

    private static void AddPlayer(
        List<LiveClientPlayer> players,
        string? championId,
        string role,
        string team,
        int roleIndex,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(championId))
        {
            return;
        }

        var rawName = (roleIndex + (team == "ORDER" ? 0 : 1)) % 2 == 0
            ? $"Character_{championId}_Name"
            : $"game_character_displayname_{championId}";
        players.Add(new LiveClientPlayer
        {
            RiotId = isActive
                ? ActivePlayerId
                : $"Test{team}{role}#DEV",
            DisplayName = championId,
            RawChampionName = rawName,
            Position = role switch
            {
                "MID" => "MIDDLE",
                "ADC" => "BOTTOM",
                "SUPPORT" => "UTILITY",
                _ => role
            },
            Team = team
        });
    }
}
