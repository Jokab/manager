namespace ManagerGame.Core.Domain;

public record TeamName
{
    public TeamName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Name = name;
    }

    public string Name { get; private set; }
}