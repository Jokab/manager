namespace ManagerGame.Domain;

public class Team : Entity
{
    private Team(Guid id,
        TeamName name,
        Guid managerId)
        : base(id)
    {
        Name = name;
        ManagerId = managerId;
    }

    public TeamName Name { get; init; }
    public Guid ManagerId { get; init; }

    public static Team Create(TeamName name,
        Guid managerId)
    {
        return new Team(Guid.NewGuid(), name, managerId);
    }
}

public record TeamName
{
    public TeamName(string? name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Name = name;
    }

    public string Name { get; private set; }
}