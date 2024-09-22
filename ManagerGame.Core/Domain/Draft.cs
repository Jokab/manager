namespace ManagerGame.Core.Domain;

public class Draft : Entity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Draft(Guid id) : base(id)
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Draft(Guid id,
        League league,
        DraftOrder draftOrder) : base(id)
    {
        if (league.Teams.Count < 2) throw new ArgumentException("Too few teams to draft");
        League = league;
        DraftOrder = draftOrder;
        State = State.Created;
    }

    public League League { get; set; }
    public Guid LeagueId { get; set; }
    public State State { get; set; }

    private DraftOrder DraftOrder { get; }

    public static Draft DoubledPeakTraversalDraft(League league) => new(Guid.NewGuid(),
        league,
        new DraftOrder(league.Teams.ToList(), new DoubledPeakTraversalDraftOrder()));

    public Team GetNext() => DraftOrder.GetNext();

    public void Start()
    {
        State = State.Started;
    }
}

public enum State
{
    Created,
    Started
}
