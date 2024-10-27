namespace ManagerGame.Core.Teams;

public class CreateTeamCommandHandler(IRepository<Manager> managerRepo, IRepository<Team> teamRepo)
    : ICommandHandler<CreateTeamCommand, Team>
{
    public async Task<Result<Team>> Handle(CreateTeamCommand command,
        CancellationToken cancellationToken = default)
    {
        var manager = await managerRepo.Find(command.ManagerId, cancellationToken);
        if (manager == null) return Result<Team>.Failure(Error.NotFound);
        var team = Team.Create(command.Name, manager.Id, [], null);

        manager.AddTeam(team);

        var createdTeam = await teamRepo.Add(team, cancellationToken);

        return Result<Team>.Success(createdTeam);
    }
}
