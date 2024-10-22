using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Drafting;

public record CreateDraftRequest(Guid LeagueId) : ICommand<Draft>;
