namespace ManagerGame.Core.Teams;

public class CreateTeamCommandHandler(ApplicationDbContext dbContext)
    : ICommandHandler<CreateTeamCommand, Team>
{
    public async Task<Result<Team>> Handle(CreateTeamCommand command,
        CancellationToken cancellationToken = default)
    {
        var manager = await dbContext.Managers2.Find(command.ManagerId, cancellationToken);
        if (manager == null) return Result<Team>.Failure(Error.NotFound);
        var team = Team.Create(command.Name, manager.Id, [], null);

        manager.AddTeam(team);

        dbContext.Teams2.Add(team);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Team>.Success(team);
    }
}
