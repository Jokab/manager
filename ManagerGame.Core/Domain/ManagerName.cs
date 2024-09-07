namespace ManagerGame.Core.Domain;

public record ManagerName
{
    public ManagerName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Name = name;
    }

    public string Name { get; private set; }
}