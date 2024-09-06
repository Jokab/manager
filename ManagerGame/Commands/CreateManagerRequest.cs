using ManagerGame.Domain;

namespace ManagerGame.Commands;

public class CreateManagerRequest : ICommand<Manager>
{
    public ManagerName Name { get; set; }
    public Email Email { get; set; }
}