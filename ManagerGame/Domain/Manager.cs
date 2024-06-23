namespace ManagerGame.Domain;

public class Manager : Entity
{
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
    
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Team> Teams { get; init; } = [];
}