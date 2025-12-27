namespace ManagerGame.Core.Teams;

public class SignPlayerCommandHandler(IRepository<Player> playerRepo, IRepository<Team> teamRepo)
    : ICommandHandler<SignPlayerRequest, Team>
{
    public async Task<Result<Team>> Handle(SignPlayerRequest command,
        CancellationToken cancellationToken = default)
    {
        var team = await teamRepo.Find(command.TeamId, cancellationToken);
        if (team is null) return Result<Team>.Failure(Error.NotFound);

        var player = await playerRepo.Find(command.PlayerId, cancellationToken);
        if (player is null) return Result<Team>.Failure(Error.NotFound);

        try
        {
            team.SignPlayer(player);
            // Update the player first since its TeamId was changed
            await playerRepo.Update(player, cancellationToken);
            // Then update the team with the new player
            await teamRepo.Update(team, cancellationToken);
        }
        catch (Exception e)
        {
            return Result<Team>.Failure(e.Message);
        }

        return Result<Team>.Success(team);
    }
}
