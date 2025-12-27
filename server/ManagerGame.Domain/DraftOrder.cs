namespace ManagerGame.Domain;

public class DraftOrder
{
    private IDraftStrategy _draftStrategy = new DoubledPeakTraversalDraftOrder();
    private int _current;
    private int _previous;

    public DraftOrder(IDraftStrategy draftStrategy)
    {
        _draftStrategy = draftStrategy;
        _current = 0;
        _previous = 0;
    }

    public DraftOrder() { }

    public Guid PeekNextTeamId(Guid[] teamIds)
    {
        if (teamIds.Length == 0) throw new ArgumentException("No teams in draft", nameof(teamIds));
        var (next, _, _) = _draftStrategy.GetNext(_current, _previous, teamIds);
        return next;
    }

    public Guid AdvanceAndGetNextTeamId(Guid[] teamIds)
    {
        if (teamIds.Length == 0) throw new ArgumentException("No teams in draft", nameof(teamIds));
        (var next, _current, _previous) = _draftStrategy.GetNext(_current, _previous, teamIds);
        return next;
    }
}

/// ChatGPT said this was the name for this traversal, but I can't really find anything online to support that :-)
/// Moves like: A -> B -> C -> C -> B -> A -> A -> B etc
public class DoubledPeakTraversalDraftOrder : IDraftStrategy
{
    public (Guid next, int current, int previous) GetNext(int current,
        int previous,
        Guid[] teamIds)
    {
        Guid next;
        if (current == 0)
        {
            if (previous == 0)
            {
                previous = current;
                next = teamIds[current++];
            }
            else
            {
                previous = current;
                next = teamIds[current];
            }
        }
        else if (current == teamIds.Length - 1)
        {
            if (previous == teamIds.Length - 1)
            {
                previous = current;
                next = teamIds[current--];
            }
            else
            {
                previous = current;
                next = teamIds[current];
            }
        }
        else
        {
            if (previous == current + 1)
            {
                previous = current;
                next = teamIds[current--];
            }
            else if (previous == current - 1)
            {
                previous = current;
                next = teamIds[current++];
            }
            else
            {
                throw new Exception("Invalid state");
            }
        }

        return (next, current, previous);
    }
}

public interface IDraftStrategy
{
    public (Guid next, int current, int previous) GetNext(int current,
        int previous,
        Guid[] teamIds);
}
