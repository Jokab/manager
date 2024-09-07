namespace ManagerGame.Core.Domain;

public class Manager : Entity
{
    private Manager(Guid id,
        ManagerName name,
        Email email)
        : base(id)
    {
        Id = id;
        Name = name;
        Email = email;
    }
    
    public static Manager Create(ManagerName name,
        Email email)
    {
        return new Manager(Guid.NewGuid(), name, email);
    }

    public ManagerName Name { get; private init; }
    public Email Email { get; private init; }
    public List<Team> Teams { get; init; } = [];

    public void AddTeam(Team team)
    {
        if (Teams.Exists(x => x.Name == team.Name))
            throw new InvalidOperationException("Team by that name already exists");
        Teams.Add(team);
    }
}