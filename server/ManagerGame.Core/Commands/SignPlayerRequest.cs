using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public record SignPlayerRequest(Guid TeamId, Guid PlayerId) : ICommand<Team>;
