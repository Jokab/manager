using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class CreateTeamRequest : ICommand<Team>
{
    public required TeamName Name { get; set; }
    public Guid ManagerId { get; set; }
}