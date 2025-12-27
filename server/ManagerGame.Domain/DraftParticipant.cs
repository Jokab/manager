namespace ManagerGame.Domain;

/// <summary>
/// Snapshotted participant list for a draft. This freezes which teams are part of the draft and in what seat order.
/// </summary>
public class DraftParticipant : Entity
{
    private DraftParticipant(Guid teamId, int seat)
    {
        TeamId = teamId;
        Seat = seat;
    }

    // EF Core constructor
    private DraftParticipant() { }

    // FK set by EF when attached to a Draft aggregate.
    public Guid DraftId { get; private set; }
    public Guid TeamId { get; private init; }
    public int Seat { get; private init; }

    internal static DraftParticipant Create(Guid teamId, int seat)
        => new(teamId, seat);
}


