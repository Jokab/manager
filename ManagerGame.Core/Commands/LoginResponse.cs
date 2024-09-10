using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public sealed record LoginResponse(Manager Manager, string Token);