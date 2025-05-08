namespace ManagerGame.Domain;

public class StartingElevenPlayer : Entity
{
    public StartingElevenPlayer(Guid id,
        StartingEleven startingEleven,
        Player player) : base(id)
    {
        StartingElevenId = startingEleven.Id;
        StartingEleven = startingEleven;
        PlayerId = player.Id;
        Player = player;
    }

    // For EF Core
    private StartingElevenPlayer(Guid id, Guid startingElevenId, Guid playerId) : base(id) =>
        (StartingElevenId, PlayerId) = (startingElevenId, playerId);

    public Guid StartingElevenId { get; private init; }
    public virtual StartingEleven StartingEleven { get; private init; } = null!;

    public Guid PlayerId { get; private init; }
    public virtual Player Player { get; private init; } = null!;
}
