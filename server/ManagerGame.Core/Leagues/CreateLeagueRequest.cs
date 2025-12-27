namespace ManagerGame.Core.Leagues;

public class CreateLeagueRequest
{
    public required string Name { get; init; }
    public int MaxPlayersFromSameCountry { get; init; } = 4;
    public int PointsPerGoal { get; init; } = 3;
    public int PointsPerWin { get; init; } = 1;
    public int PointsPerAssist { get; init; } = 1;
    public int PointsPerCleanSheet { get; init; } = 1;
}
