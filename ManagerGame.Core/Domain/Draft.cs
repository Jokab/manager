namespace ManagerGame.Core.Domain;

public class Draft : Entity
{
    public League League { get; set; }
    public Guid LeagueId { get; set; }

    private DraftOrder DraftOrder { get; }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Draft(Guid id) : base(id)
    {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Draft(Guid id, 
        League league,
        DraftOrder draftOrder) : base(id)
    {
        if (league.Teams.Count < 2) throw new ArgumentException("Too few teams to draft");
        League = league;
        DraftOrder = draftOrder;
    }

    public static Draft DoubledPeakTraversalDraft(League league)
    {
        return new Draft(Guid.NewGuid(), league, new DraftOrder(league.Teams.ToList(), new DoubledPeakTraversalDraftOrder()));
    }

    public Team GetNext()
    {
        return DraftOrder.GetNext();
    }
}