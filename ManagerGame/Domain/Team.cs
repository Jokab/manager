namespace ManagerGame.Domain;

public class Team(string name, Guid managerId) : Entity
{
    public string Name { get; init; } = name;
    public Guid ManagerId { get; init; } = managerId;
}