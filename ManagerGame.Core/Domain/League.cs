using System.Text.Json.Serialization;

namespace ManagerGame.Core.Domain;

public class League : Entity
{
    public ICollection<Team> Teams { get; init; }
    public ICollection<Draft> Drafts { get; init; }
    
    [JsonConstructor]
    public League(Guid id) : base(id)
    {
        Teams = [];
        Drafts = [];
    }

    public static League Empty()
    {
        return new League(Guid.NewGuid());
    }

    public void AddTeam(Team team)
    {
        // if (Teams.Any(x => x.Name == team.Name))
        // {
        //     throw new ArgumentException("Team with name already joined");
        // }
        Teams.Add(team);
    }
}
