using ManagerGame.Core.Domain;

namespace ManagerGame;

public sealed record LoginResponse(Manager Manager, string Token);