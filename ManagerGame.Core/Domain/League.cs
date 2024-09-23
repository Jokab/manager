using System.Text.Json.Serialization;

namespace ManagerGame.Core.Domain;

public class League : Entity
{
    [JsonConstructor]
    public League(Guid id) : base(id)
    {
        Teams = [];
        Drafts = [];
    }

    public ICollection<Team> Teams { get; init; }
    public ICollection<Draft> Drafts { get; init; }

    public static League Empty() => new(Guid.NewGuid());

    public void AdmitTeam(Team team)
    {
        // if (Teams.Any(x => x.Name == team.Name))
        // {
        //     throw new ArgumentException("Team with name already joined");
        // }
        Teams.Add(team);
    }

    public void CreateDraft()
    {
        Drafts.Add(Draft.DoubledPeakTraversalDraft(this));
    }
}
