using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class CreateTeamRequest : ICommand<Team>
{
    public TeamName Name { get; set; }
    public Guid ManagerId { get; set; }
}