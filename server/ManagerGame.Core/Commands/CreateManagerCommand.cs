using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class CreateManagerCommand : ICommand<Manager>
{
    public required ManagerName Name { get; init; }
    public required Email Email { get; init; }
}
