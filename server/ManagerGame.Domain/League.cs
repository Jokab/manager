using System.Text.Json.Serialization;

namespace ManagerGame.Domain;

public class League : Entity
{
    [JsonConstructor]
    private League(Guid id) : base(id)
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
        if (Drafts.Any(x => x.State != DraftState.Finished))
            throw new InvalidOperationException("Cannot create new draft while there unfinished drafts");
        Drafts.Add(Draft.DoubledPeakTraversalDraft(this));
    }
}
