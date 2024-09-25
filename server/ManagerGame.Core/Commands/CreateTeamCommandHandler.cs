using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class CreateTeamCommandHandler(ApplicationDbContext dbContext) : ICommandHandler<CreateTeamCommand, Team>
{
    public async Task<Result<Team>> Handle(CreateTeamCommand command,
        CancellationToken cancellationToken)
    {
        var manager = await dbContext.Managers.FindAsync([command.ManagerId], cancellationToken);
        if (manager == null) return Result<Team>.Failure(Error.NotFound);
        var team = Team.Create(command.Name, manager.Id, [], null);

        manager.AddTeam(team);

        var createdTeam = dbContext.Add(team);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Team>.Success(createdTeam.Entity);
    }
}
