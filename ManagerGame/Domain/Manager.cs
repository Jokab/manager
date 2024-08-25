using System.ComponentModel.DataAnnotations;

namespace ManagerGame.Domain;

public class Manager : Entity
{
    [Key]
    public Guid Id { get; private init; }
    public string Name { get; private init; }
    public string Email { get; private init; }
    public List<Team> Teams { get; init; } = [];

    public Manager(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public void AddTeam(Team team)
    {
        if (Teams.Exists(x => x.Name == team.Name))
        {
            throw new InvalidOperationException("Team by that name already exists");
        }
        Teams.Add(team);
    }
}