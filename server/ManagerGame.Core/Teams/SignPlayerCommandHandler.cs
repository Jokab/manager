namespace ManagerGame.Core.Teams;

public class SignPlayerCommandHandler(IRepository<Player> playerRepo, IRepository<Team> teamRepo) : ICommandHandler<SignPlayerRequest, Team>
{
    public async Task<Result<Team>> Handle(SignPlayerRequest command,
        CancellationToken cancellationToken = default)
    {
        Team? team = await teamRepo.Find(command.TeamId, cancellationToken);
        if (team is null) return Result<Team>.Failure(Error.NotFound);

        Player? player = await playerRepo.Find(command.PlayerId, cancellationToken);
        if (player is null) return Result<Team>.Failure(Error.NotFound);
        if (player.IsSigned) return Result<Team>.Failure(Error.PlayerAlreadySigned);

        team.SignPlayer(player);

        await teamRepo.Update(team, cancellationToken);

        return Result<Team>.Success(team);
    }
}
