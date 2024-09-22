namespace ManagerGame.Core.Domain;

public class League : Entity
{
    public League(Guid id, ICollection<Team> teams, ICollection<Draft> drafts) : base(id)
    {
        Teams = teams;
        Drafts = drafts;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private League(Guid id): base(id) {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static League Empty()
    {
        return new League(Guid.NewGuid(), [], []);
    }

    public ICollection<Team> Teams { get; init; }
    public ICollection<Draft> Drafts { get; init; }
}
