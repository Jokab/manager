using ManagerGame.Domain;

namespace ManagerGame.Commands;

public class CreateTeamRequest : ICommand<Team>
{
    public TeamName Name { get; set; }
    public Guid ManagerId { get; set; }
}