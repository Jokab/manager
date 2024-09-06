using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class CreateManagerRequest : ICommand<Manager>
{
    public ManagerName Name { get; set; }
    public Email Email { get; set; }
}