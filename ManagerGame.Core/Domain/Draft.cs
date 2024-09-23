namespace ManagerGame.Core.Domain;

public class Draft : Entity
{
    private Draft(Guid id,
        Guid leagueId,
        ICollection<Team> teams,
        DraftOrder draftOrder) : base(id)
    {
        LeagueId = leagueId;
        Teams = teams;
        DraftOrder = draftOrder;
        State = State.Created;
    }

    public Guid LeagueId { get; set; }
    public ICollection<Team> Teams { get; }
    public State State { get; private set; }

    private DraftOrder DraftOrder { get; }

    public static Draft DoubledPeakTraversalDraft(League league) => new(
        Guid.NewGuid(),
        league.Id,
        league.Teams,
        new DraftOrder(league.Teams.ToList(), new DoubledPeakTraversalDraftOrder()));

    public Team GetNext() => DraftOrder.GetNext();

    public void Start()
    {
        if (Teams.Count < 2) throw new ArgumentException("Too few teams to draft");
        State = State.Started;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Draft(Guid id) : base(id)
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public enum State
{
    Created,
    Started
}
