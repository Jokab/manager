using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Domain;

public class ManagerTest
{
    [Fact]
    public void CanAddSeparateTeams()
    {
        var manager = Manager.Create(new ManagerName("Jakob"), new Email("jakob@jakobsson.com"));
        
        manager.AddTeam(Team.Create(new TeamName("Laget"), manager.Id));
        manager.AddTeam(Team.Create(new TeamName("Laget2"), manager.Id));

        Assert.Contains(manager.Teams, x => x.Name.Name == "Laget");
        Assert.Contains(manager.Teams, x => x.Name.Name == "Laget2");
    }
    
    [Fact]
    public void CannotAddDuplicateTeams()
    {
        var manager = Manager.Create(new ManagerName("Jakob"), new Email("jakob@jakobsson.com"));
        
        manager.AddTeam(Team.Create(new TeamName("Laget"), manager.Id));
        
        Assert.Throws<ArgumentException>(() => manager.AddTeam(Team.Create(new TeamName("Laget"), manager.Id)));
    }
}