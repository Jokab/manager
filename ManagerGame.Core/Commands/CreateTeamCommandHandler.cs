using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class CreateTeamCommandHandler(ApplicationDbContext dbContext) : ICommandHandler<CreateTeamRequest, Team>
{
    public async Task<Result<Team>> Handle(CreateTeamRequest request,
        CancellationToken cancellationToken)
    {
        var manager = await dbContext.Managers.FindAsync([request.ManagerId], cancellationToken);
        if (manager == null) return Result<Team>.Failure(Error.NotFound);
        var team = Team.Create(request.Name, manager.Id, [], new League(Guid.Empty, new List<Team>(), new List<Draft>()));

        manager.AddTeam(team);

        var createdTeam = dbContext.Add(team);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Team>.Success(createdTeam.Entity);
    }
}
