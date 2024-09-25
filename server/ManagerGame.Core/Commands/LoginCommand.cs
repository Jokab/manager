using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class LoginCommand : ICommand<LoginResponse>
{
    public Email? ManagerEmail { get; init; }
}
