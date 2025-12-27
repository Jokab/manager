using OneOf;
using OneOf.Types;

namespace ManagerGame.Domain;

public class Draft : Entity
{
    private Draft(Guid id,
        League league,
        DraftOrder draftOrder) : base(id)
    {
        LeagueId = league.Id;
        League = league;
        DraftOrder = draftOrder;
        State = DraftState.Created;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Draft(Guid id) : base(id)
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Guid LeagueId { get; private init; }

    public League League { get; private init; }
    public DraftState State { get; private set; }
    private DraftOrder DraftOrder { get; }
    public ICollection<Team> Teams => League.Teams;

    public static Draft DoubledPeakTraversalDraft(League league)
    {
        if (league.Teams.Count == 0) throw new ArgumentException("No teams in league", nameof(league));

        return new Draft(
            Guid.NewGuid(),
            league,
            new DraftOrder(league.Teams.ToList(), new DoubledPeakTraversalDraftOrder()));
    }

    public OneOf<Team, None> GetNext()
    {
        if (State is DraftState.Created)
        {
            return new None();
        }

        return DraftOrder.GetNext();
    }

    public void Start()
    {
        if (State is DraftState.Started) throw new InvalidOperationException("Draft is already started");
        const int minimumTeamCount = 2;
        if (Teams.Count < minimumTeamCount)
            throw new ArgumentException($"Too few teams to draft, needs at least {minimumTeamCount}");
        State = DraftState.Started;
    }
}

public enum DraftState
{
    Created,
    Started,
    Finished
}

public class StartedDraft
{

}
