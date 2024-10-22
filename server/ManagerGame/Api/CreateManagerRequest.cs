using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api;

public record CreateManagerRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }

    public CreateManagerCommand ToCommand() => new() { Email = new Email(Email!), Name = new ManagerName(Name!) };
}
