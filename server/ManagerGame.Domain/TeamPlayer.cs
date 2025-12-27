namespace ManagerGame.Domain;

public class TeamPlayer : Entity
{
    public TeamPlayer(Guid id, Team team, Player player) : base(id)
    {
        LeagueId = team.LeagueId;
        TeamId = team.Id;
        Team = team;
        PlayerId = player.Id;
        Player = player;
    }

    // ReSharper disable once UnusedMember.Local used by EF core
    private TeamPlayer(Guid id, Guid leagueId, Guid teamId, Guid playerId) : base(id) =>
        (LeagueId, TeamId, PlayerId) = (leagueId, teamId, playerId);

    public Guid LeagueId { get; private set; }

    public Guid TeamId { get; private set; }
    public virtual Team Team { get; private set; } = null!;

    public Guid PlayerId { get; private set; }
    public virtual Player Player { get; private set; } = null!;
};
