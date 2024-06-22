namespace ManagerGame;

public class CreateManagerRequest : ICommand
{
    public string Name { get; set; }
    public string Email { get; set; }
}