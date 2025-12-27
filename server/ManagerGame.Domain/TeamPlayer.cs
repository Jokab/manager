namespace ManagerGame.Domain;

public class TeamPlayer : Entity
{
    public TeamPlayer(Team team, Player player)
    {
        LeagueId = team.LeagueId;
        TeamId = team.Id;
        Team = team;
        PlayerId = player.Id;
        Player = player;
    }

    // ReSharper disable once UnusedMember.Local used by EF core
    private TeamPlayer() =>
        (LeagueId, TeamId, PlayerId) = (Guid.Empty, Guid.Empty, Guid.Empty);

    public Guid LeagueId { get; private set; }

    public Guid TeamId { get; private set; }
    public virtual Team Team { get; private set; } = null!;

    public Guid PlayerId { get; private set; }
    public virtual Player Player { get; private set; } = null!;
};
