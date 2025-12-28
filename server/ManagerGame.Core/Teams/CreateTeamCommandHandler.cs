namespace ManagerGame.Core.Teams;

public class CreateTeamCommandHandler(ApplicationDbContext dbContext)
    : ICommandHandler<CreateTeamCommand, Team>
{
    public async Task<Result<Team>> Handle(CreateTeamCommand command,
        CancellationToken cancellationToken = default)
    {
        var league = await dbContext.LeaguesRepo.Find(command.LeagueId, cancellationToken);
        if (league is null) return Result<Team>.Failure(Error.NotFound);

        var manager = await dbContext.ManagersRepo.Find(command.ManagerId, cancellationToken);
        if (manager == null) return Result<Team>.Failure(Error.NotFound);
        var team = Team.Create(command.Name, manager.Id, [], league.Id);

        league.AdmitTeam(team);
        manager.AddTeam(team);

        dbContext.TeamsRepo.Add(team);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Team>.Success(team);
    }
}
