using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class SignPlayerRequest(Guid teamId) : ICommand<Team>
{
	public Guid TeamId { get; set; } = teamId;
}
