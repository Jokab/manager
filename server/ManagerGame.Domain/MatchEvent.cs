namespace ManagerGame.Domain;

public class MatchEvent : Entity
{
    public MatchEvent(
        Guid id,
        Guid matchId,
        Guid playerId,
        MatchEventType eventType,
        bool isHomeTeam,
        int minute) : base(id)
    {
        MatchId = matchId;
        PlayerId = playerId;
        EventType = eventType;
        IsHomeTeam = isHomeTeam;
        Minute = minute;
    }

    public Guid MatchId { get; private set; }
    public MatchResult Match { get; set; } = null!;

    public Guid PlayerId { get; private set; }
    public Player Player { get; set; } = null!;

    public MatchEventType EventType { get; private set; }
    public bool IsHomeTeam { get; private set; }
    public int Minute { get; private set; }
}

public enum MatchEventType
{
    Goal,
    Assist,
    YellowCard,
    RedCard,
    CleanSheet
}
