namespace ManagerGame.Core.Managers;

public class LoginCommand : ICommand<LoginResponse>
{
    public Email? ManagerEmail { get; init; }
}
