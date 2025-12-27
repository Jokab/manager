namespace ManagerGame.Domain;

/// <summary>
/// Records a single pick made during a draft.
/// Owned by the <see cref="Draft"/> aggregate.
/// </summary>
public class DraftPick : Entity
{
    private DraftPick(Guid draftId, int pickNumber, Guid teamId, Guid playerId)
    {
        DraftId = draftId;
        PickNumber = pickNumber;
        TeamId = teamId;
        PlayerId = playerId;
        PickedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private DraftPick() { }

    public Guid DraftId { get; private init; }
    public int PickNumber { get; private init; }
    public Guid TeamId { get; private init; }
    public Guid PlayerId { get; private init; }
    public DateTime PickedAt { get; private init; }

    internal static DraftPick Create(Guid draftId, int pickNumber, Guid teamId, Guid playerId)
        => new(draftId, pickNumber, teamId, playerId);
}

