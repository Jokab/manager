namespace ManagerGame.Core.Commands;

public class LoginRequest : ICommand<LoginResponse>
{
    public Guid ManagerId { get; init; }
}
