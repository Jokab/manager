namespace ManagerGame.Core.Teams;

public class SignPlayerCommandHandler(
    ApplicationDbContext dbContext)
    : ICommandHandler<SignPlayerRequest, Team>
{
    public async Task<Result<Team>> Handle(SignPlayerRequest command,
        CancellationToken cancellationToken = default)
    {
        var team = await dbContext.Teams2.Find(command.TeamId, cancellationToken);
        if (team is null) return Result<Team>.Failure(Error.NotFound);

        var player = await dbContext.Players2.Find(command.PlayerId, cancellationToken);
        if (player is null) return Result<Team>.Failure(Error.NotFound);

        try
        {
            team.SignPlayer(player);
            // Persist the join entity explicitly.
            // Relying on aggregate graph updates has proven brittle across providers (SQLite/Postgres),
            // and can lead to UPDATEs against non-existent rows.
            var teamPlayer = team.Players.Last();
            dbContext.TeamPlayers2.Add(teamPlayer);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return Result<Team>.Failure(e.Message);
        }

        return Result<Team>.Success(team);
    }
}
