namespace ManagerGame.Domain;

public class StartingElevenPlayer : Entity
{
    public StartingElevenPlayer(
        StartingEleven startingEleven,
        Player player)
    {
        StartingElevenId = startingEleven.Id;
        StartingEleven = startingEleven;
        PlayerId = player.Id;
        Player = player;
    }

    // For EF Core
    private StartingElevenPlayer() =>
        (StartingElevenId, PlayerId) = (Guid.Empty, Guid.Empty);

    public Guid StartingElevenId { get; private init; }
    public virtual StartingEleven StartingEleven { get; private init; } = null!;

    public Guid PlayerId { get; private init; }
    public virtual Player Player { get; private init; } = null!;
}
