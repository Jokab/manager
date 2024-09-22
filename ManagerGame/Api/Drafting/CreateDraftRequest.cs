using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Drafting;

public record CreateDraftRequest(Guid LeagueId) : ICommand<Draft>;