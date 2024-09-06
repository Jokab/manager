using ManagerGame.Core.Commands;

namespace ManagerGame;

public class LoginRequest : ICommand<LoginResponse>
{
    public Guid ManagerId { get; set; }
}