using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Teams;

public record SignPlayerRequest(Guid TeamId, Guid PlayerId) : ICommand<Team>;
