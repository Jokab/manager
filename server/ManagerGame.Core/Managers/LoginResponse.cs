using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Managers;

public sealed record LoginResponse(Manager Manager, string Token);
