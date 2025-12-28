namespace ManagerGame.Core.Leagues;

public class AdmitTeamHandler(ApplicationDbContext dbContext)
    : ICommandHandler<AdmitTeamRequest, League>

{
    public async Task<Result<League>> Handle(AdmitTeamRequest command,
        CancellationToken cancellationToken = default)
    {
        var league = await dbContext.LeaguesRepo.Find(command.LeagueId, cancellationToken);
        if (league is null) return Result<League>.Failure(Error.NotFound);
        var team = await dbContext.TeamsRepo.Find(command.TeamId, cancellationToken);
        if (team is null) return Result<League>.Failure(Error.NotFound);

        league.AdmitTeam(team);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<League>.Success(league);
    }
}
