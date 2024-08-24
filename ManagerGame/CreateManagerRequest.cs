namespace ManagerGame;

public class CreateManagerRequest : ICommand<Manager>
{
    public string Name { get; set; }
    public string Email { get; set; }
}
