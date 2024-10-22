namespace ManagerGame.Core.Teams;

public class CreateTeamRequest
{
    public string? Name { get; init; }
    public Guid? ManagerId { get; init; }
}

public class CreateTeamCommand : ICommand<Team>
{
    public required TeamName Name { get; init; }
    public Guid ManagerId { get; init; }
}
