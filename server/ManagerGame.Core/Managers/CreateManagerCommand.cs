namespace ManagerGame.Core.Managers;

public class CreateManagerCommand
{
    public required ManagerName Name { get; init; }
    public required Email Email { get; init; }
}
