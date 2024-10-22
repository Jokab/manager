namespace ManagerGame.Core.Domain;

public class DraftOrder
{
    private readonly IDraftStrategy _draftStrategy;
    private readonly Team[] _teams;
    private int _current;
    private int _previous;

    public DraftOrder(List<Team> teams,
        IDraftStrategy draftStrategy)
    {
        _draftStrategy = draftStrategy;
        _current = 0;
        _teams = teams.ToArray();
    }

    public Team GetNext()
    {
        (var next, _current, _previous) = _draftStrategy.GetNext(_current, _previous, _teams);
        return next;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DraftOrder() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

/// ChatGPT said this was the name for this traversal, but I can't really find anything online to support that :-)
/// Moves like: A -> B -> C -> C -> B -> A -> A -> B etc
public class DoubledPeakTraversalDraftOrder : IDraftStrategy
{
    public (Team next, int current, int previous) GetNext(int current,
        int previous,
        Team[] teams)
    {
        Team next;
        if (current == 0)
        {
            if (previous == 0)
            {
                previous = current;
                next = teams[current++];
            }
            else
            {
                previous = current;
                next = teams[current];
            }
        }
        else if (current == teams.Length - 1)
        {
            if (previous == teams.Length - 1)
            {
                previous = current;
                next = teams[current--];
            }
            else
            {
                previous = current;
                next = teams[current];
            }
        }
        else
        {
            if (previous == current + 1)
            {
                previous = current;
                next = teams[current--];
            }
            else if (previous == current - 1)
            {
                previous = current;
                next = teams[current++];
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
    public (Team next, int current, int previous) GetNext(int current,
        int previous,
        Team[] teams);
}
