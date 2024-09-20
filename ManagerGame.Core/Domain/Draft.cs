namespace ManagerGame.Core.Domain;

public class Draft
{
    public List<Team> Teams { get; set; }
    private readonly IDraftOrder _draftOrder; 

    private Draft(List<Team> teams,
        IDraftOrder draftOrder)
    {
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
    
    private interface IDraftOrder
    {
        public Team GetNext();
    }

    /// Chat GPT said this was the name for this traversal, but I can't really find anything online to support that :-)
    /// Moves like: A -> B -> C -> C -> B -> A -> A -> B etc
    private class DoubledPeakTraversalDraftOrder : IDraftOrder
    {
        private readonly Team[] _teams;
        private int _current;
        private int _previous;

        public DoubledPeakTraversalDraftOrder(List<Team> teams)
        {
            _current = 0;
            _teams = teams.ToArray();
        }

        public Team GetNext()
        {
            if (_current == 0)
            {
                if (_previous == 0)
                {
                    _previous = _current;
                    return _teams[_current++];
                }
                else
                {
                    _previous = _current;
                    return _teams[_current];
                }
            }
            
            if (_current == _teams.Length - 1)
            {
                if (_previous == _teams.Length - 1)
                {
                    _previous = _current;
                    return _teams[_current--];
                }
                else
                {
                    _previous = _current;
                    return _teams[_current];
                }
            }

            return _previous == _current + 1 ? _teams[_current--] : _teams[_current++];
        }
    }
}

