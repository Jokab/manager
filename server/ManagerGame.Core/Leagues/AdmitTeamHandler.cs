namespace ManagerGame.Core.Leagues;

public class AdmitTeamHandler(IRepository<Team> teamRepo, IRepository<League> leagueRepo)
    : ICommandHandler<AdmitTeamRequest, League>

{
    public async Task<Result<League>> Handle(AdmitTeamRequest command,
        CancellationToken cancellationToken = default)
    {
        League? league = await leagueRepo.Find(command.LeagueId, cancellationToken);
        if (league is null) return Result<League>.Failure(Error.NotFound);
        Team? team = await teamRepo.Find(command.TeamId, cancellationToken);
        if (team is null) return Result<League>.Failure(Error.NotFound);

        league.AdmitTeam(team);

        League updatedLeague = await leagueRepo.Update(league, cancellationToken);

        return Result<League>.Success(updatedLeague);
    }
}
