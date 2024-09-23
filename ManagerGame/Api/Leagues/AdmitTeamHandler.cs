using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Leagues;

internal class AdmitTeamHandler(IRepository<Team> teamRepo, IRepository<League> leagueRepo)
    : ICommandHandler<AdmitTeamRequest, League>
{
    public async Task<Result<League>> Handle(AdmitTeamRequest command,
        CancellationToken cancellationToken = default)
    {
        var league = await leagueRepo.Find(command.LeagueId, cancellationToken);
        if (league is null) return Result<League>.Failure(Error.NotFound);
        var team = await teamRepo.Find(command.TeamId, cancellationToken);
        if (team is null) return Result<League>.Failure(Error.NotFound);

        league.AdmitTeam(team);

        var updatedLeague = await leagueRepo.Update(league, cancellationToken);

        return Result<League>.Success(updatedLeague);
    }
}
