using ManagerGame.Domain;

namespace ManagerGame.Commands;

public class CreateTeamRequest : ICommand<Team>
{
    public string Name { get; set; }
    public Guid ManagerId { get; set; }
}
