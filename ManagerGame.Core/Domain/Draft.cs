namespace ManagerGame.Core.Domain;

public class Draft
{
    public List<Team> Teams { get; set; }
    private readonly DraftOrder _draftOrder;

    private Draft(List<Team> teams,
        DraftOrder draftOrder)
    {
        if (teams.Count < 2) throw new ArgumentException("Too few teams to draft");
        Teams = teams;
        _draftOrder = draftOrder;
    }

    public static Draft DoublePeakTraversalDraft(List<Team> teams)
    {
        return new Draft(teams, new DoubledPeakTraversalDraftOrder(teams));
    }

    public Team GetNext()
    {
        return _draftOrder.GetNext();
    }

    private abstract class DraftOrder
    {
        protected readonly Team[] Teams;
        protected int Current;
        protected int Previous;

        protected DraftOrder(List<Team> teams)
        {
            Current = 0;
            Teams = teams.ToArray();
        }

        public abstract Team GetNext();
    }

    /// Chat GPT said this was the name for this traversal, but I can't really find anything online to support that :-)
    /// Moves like: A -> B -> C -> C -> B -> A -> A -> B etc
    private class DoubledPeakTraversalDraftOrder(List<Team> teams) : DraftOrder(teams)
    {
        public override Team GetNext()
        {
            Team next;
            if (Current == 0)
            {
                if (Previous == 0)
                {
                    Previous = Current;
                    next = Teams[Current++];
                }
                else
                {
                    Previous = Current;
                    next = Teams[Current];
                }
            }
            else if (Current == Teams.Length - 1)
            {
                if (Previous == Teams.Length - 1)
                {
                    Previous = Current;
                    next = Teams[Current--];
                }
                else
                {
                    Previous = Current;
                    next = Teams[Current];
                }
            }
            else
            {
                if (Previous == Current + 1)
                {
                    Previous = Current;
                    next = Teams[Current--];
                }
                else if (Previous == Current - 1)
                {
                    Previous = Current;
                    next = Teams[Current++];
                }
                else
                {
                    throw new Exception("Invalid state");
                }
            }

            return next;
        }
    }
}
