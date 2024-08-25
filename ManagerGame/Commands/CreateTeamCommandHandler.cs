namespace ManagerGame.Commands;

public class CreateTeamCommandHandler(ApplicationDbContext dbContext) : ICommandHandler<CreateTeamRequest, Team>
{
    public async Task<Result<Team>> Handle(CreateTeamRequest request, CancellationToken cancellationToken)
    {
        var manager = await dbContext.Managers.FindAsync([request.ManagerId], cancellationToken);
        if (manager == null)
        {
            return Result<Team>.Failure(Error.NotFound);
        }
        var team = new Team(request.Name, manager.Id);

        manager.AddTeam(team);

        dbContext.Add(team);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result<Team>.Success(team);
    }
}