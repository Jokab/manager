namespace ManagerGame.Domain;

public class TeamPlayer : Entity
{
    public TeamPlayer(Guid id, Team team, Player player) : base(id)
    {
        TeamId = team.Id;
        Team = team;
        PlayerId = player.Id;
        Player = player;
    }

    // ReSharper disable once UnusedMember.Local used by EF core
    private TeamPlayer(Guid id, Guid teamId, Guid playerId) : base(id) =>
        (TeamId, PlayerId) = (teamId, playerId);

    public Guid TeamId { get; private init; }
    public virtual Team Team { get; private init; } = null!;

    public Guid PlayerId { get; private init; }
    public virtual Player Player { get; private init; } = null!;
};
