namespace ManagerGame.Commands;

public class CreateTeamRequest : ICommand
{
    public string Name { get; set; }
    public Guid ManagerId { get; set; }
}