namespace ManagerGame.Domain;

public class MatchResult : Entity
{
    public MatchResult(
        DateTime matchDate,
        string homeTeamCountry,
        string awayTeamCountry)
    {
        MatchDate = matchDate;
        HomeTeamCountry = homeTeamCountry;
        AwayTeamCountry = awayTeamCountry;
        HomeGoals = 0;
        AwayGoals = 0;
        IsFinished = false;
        MatchEvents = [];
    }

    // For EF Core
    private MatchResult()
    {
        MatchEvents = [];
        HomeTeamCountry = string.Empty;
        AwayTeamCountry = string.Empty;
    }

    public DateTime MatchDate { get; private set; }
    public string HomeTeamCountry { get; private set; }
    public string AwayTeamCountry { get; private set; }
    public int HomeGoals { get; private set; }
    public int AwayGoals { get; private set; }
    public bool IsFinished { get; private set; }
    public ICollection<MatchEvent> MatchEvents { get; private set; }

    // Reference to League
    public Guid LeagueId { get; set; }
    public League League { get; set; } = null!;

    public void RecordResult(int homeGoals, int awayGoals)
    {
        if (IsFinished)
            throw new InvalidOperationException("Cannot change result of a finished match");

        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        IsFinished = true;

        // Set elimination status for players based on the match result
        // Implementation would depend on tournament rules
    }

    public void AddEvent(MatchEvent matchEvent)
    {
        if (IsFinished)
            throw new InvalidOperationException("Cannot add events to a finished match");

        MatchEvents.Add(matchEvent);

        // Update goals based on event type
        if (matchEvent.EventType == MatchEventType.Goal)
        {
            if (matchEvent.IsHomeTeam)
                HomeGoals++;
            else
                AwayGoals++;
        }
    }
}
