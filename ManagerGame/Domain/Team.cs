using System.ComponentModel.DataAnnotations;

namespace ManagerGame.Domain;

public class Team(string name, Guid managerId) : Entity
{
    [Key]
    public Guid Id { get; private init; }
    public string Name { get; init; } = name;
    public Guid ManagerId { get; init; } = managerId;
}