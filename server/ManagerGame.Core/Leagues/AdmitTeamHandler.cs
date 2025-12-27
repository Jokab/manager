namespace ManagerGame.Core.Leagues;

public class AdmitTeamHandler(ApplicationDbContext dbContext)
    : ICommandHandler<AdmitTeamRequest, League>

{
    public async Task<Result<League>> Handle(AdmitTeamRequest command,
        CancellationToken cancellationToken = default)
    {
        var league = await dbContext.Leagues2.Find(command.LeagueId, cancellationToken);
        if (league is null) return Result<League>.Failure(Error.NotFound);
        var team = await dbContext.Teams2.Find(command.TeamId, cancellationToken);
        if (team is null) return Result<League>.Failure(Error.NotFound);

        league.AdmitTeam(team);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<League>.Success(league);
    }
}
