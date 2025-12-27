namespace ManagerGame.Domain;

/// <summary>
/// Snapshotted participant list for a draft. This freezes which teams are part of the draft and in what seat order.
/// </summary>
public class DraftParticipant : Entity
{
    private DraftParticipant(Guid id, Guid draftId, Guid teamId, int seat) : base(id)
    {
        DraftId = draftId;
        TeamId = teamId;
        Seat = seat;
    }

    // EF Core constructor
    private DraftParticipant(Guid id) : base(id) { }

    public Guid DraftId { get; private init; }
    public Guid TeamId { get; private init; }
    public int Seat { get; private init; }

    internal static DraftParticipant Create(Guid draftId, Guid teamId, int seat)
        => new(Guid.NewGuid(), draftId, teamId, seat);
}


