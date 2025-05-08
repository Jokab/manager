namespace ManagerGame.Domain;

public class LeagueSettings : Entity
{
    public LeagueSettings(Guid id) : base(id)
    {
        MaxPlayersFromSameCountry = 4;
        PointsPerGoal = 3;
        PointsPerWin = 1;
        PointsPerAssist = 1;
        PointsPerCleanSheet = 1;
    }

    public LeagueSettings(
        Guid id,
        int maxPlayersFromSameCountry,
        int pointsPerGoal,
        int pointsPerWin,
        int pointsPerAssist,
        int pointsPerCleanSheet) : base(id)
    {
        MaxPlayersFromSameCountry = maxPlayersFromSameCountry;
        PointsPerGoal = pointsPerGoal;
        PointsPerWin = pointsPerWin;
        PointsPerAssist = pointsPerAssist;
        PointsPerCleanSheet = pointsPerCleanSheet;
    }

    public int MaxPlayersFromSameCountry { get; private set; }
    public int PointsPerGoal { get; private set; }
    public int PointsPerWin { get; private set; }
    public int PointsPerAssist { get; private set; }
    public int PointsPerCleanSheet { get; private set; }

    // Add reference to League
    public Guid LeagueId { get; set; }
    public League League { get; set; } = null!;
}
