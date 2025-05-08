using ManagerGame.Core.Managers;

namespace ManagerGame.Api;

public record RegisterManagerRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }

    public RegisterManagerCommand ToCommand() => new() { Email = new Email(Email!), Name = new ManagerName(Name!) };
}
