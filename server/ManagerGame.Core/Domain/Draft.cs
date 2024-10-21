namespace ManagerGame.Core.Domain;

public class Draft : Entity
{
    private Draft(Guid id,
        League league,
        DraftOrder draftOrder) : base(id)
    {
        LeagueId = league.Id;
        League = league;
        DraftOrder = draftOrder;
        State = State.Created;
    }

    public Guid LeagueId { get; set; }
    public League League { get; set; }
    public ICollection<Team> Teams => League.Teams;
    public State State { get; private set; }

    private DraftOrder DraftOrder { get; }

    public static Draft DoubledPeakTraversalDraft(League league) => new(
        Guid.NewGuid(),
        league,
        new DraftOrder(league.Teams.ToList(), new DoubledPeakTraversalDraftOrder()));

    public Team GetNext() => DraftOrder.GetNext();

    public void Start()
    {
        if (State is State.Started)
        {
            throw new InvalidOperationException("Draft is already started");
        }
        const int minimumTeamCount = 2;
        if (Teams.Count < minimumTeamCount) throw new ArgumentException($"Too few teams to draft, needs at least {minimumTeamCount}");
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
    Started,
    Finished
}
