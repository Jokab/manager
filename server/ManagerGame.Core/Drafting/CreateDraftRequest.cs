namespace ManagerGame.Core.Drafting;

public record CreateDraftRequest(Guid LeagueId) : ICommand<Draft>;
