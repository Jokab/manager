namespace ManagerGame.Core.Teams;

public class CreateTeamCommand
{
    public required TeamName Name { get; init; }
    public Guid ManagerId { get; init; }
}
