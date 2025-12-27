namespace ManagerGame.Core.Teams;

public interface ITeamSigningService
{
    Task<Result<Team>> SignPlayer(Guid teamId, Guid playerId, CancellationToken cancellationToken = default);
}


