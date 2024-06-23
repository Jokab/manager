using ManagerGame.Domain;

namespace ManagerGame.Commands;

public class CreateManagerRequest : ICommand
{
    public string Name { get; set; }
    public string Email { get; set; }
}