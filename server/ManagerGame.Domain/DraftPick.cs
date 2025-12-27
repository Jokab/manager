namespace ManagerGame.Domain;

/// <summary>
/// Records a single pick made during a draft.
/// Owned by the <see cref="Draft"/> aggregate.
/// </summary>
public class DraftPick : Entity
{
    private DraftPick(Guid id, Guid draftId, int pickNumber, Guid teamId, Guid playerId) : base(id)
    {
        DraftId = draftId;
        PickNumber = pickNumber;
        TeamId = teamId;
        PlayerId = playerId;
        PickedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private DraftPick(Guid id) : base(id) { }

    public Guid DraftId { get; private init; }
    public int PickNumber { get; private init; }
    public Guid TeamId { get; private init; }
    public Guid PlayerId { get; private init; }
    public DateTime PickedAt { get; private init; }

    internal static DraftPick Create(Guid draftId, int pickNumber, Guid teamId, Guid playerId)
        // Use default key so EF Core will treat this as a new entity when it is discovered via a tracked aggregate
        // navigation and will generate the Guid key when tracking it.
        => new(Guid.Empty, draftId, pickNumber, teamId, playerId);
}

