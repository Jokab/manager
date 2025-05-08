namespace ManagerGame.Core.Managers;

public class RegisterManagerCommand
{
    public required ManagerName Name { get; init; }
    public required Email Email { get; init; }
}
