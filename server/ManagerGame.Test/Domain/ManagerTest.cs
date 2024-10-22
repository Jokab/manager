namespace ManagerGame.Test.Domain;

public class ManagerTest
{
    [Fact]
    public void CanAddSeparateTeams()
    {
        var manager = Manager.Create(new ManagerName("Jakob"), new Email("jakob@jakobsson.com"));

        manager.AddTeam(TestData.TeamEmpty("Laget"));
        manager.AddTeam(TestData.TeamEmpty("Laget2"));

        Assert.Contains(manager.Teams, x => x.Name.Name == "Laget");
        Assert.Contains(manager.Teams, x => x.Name.Name == "Laget2");
    }

    [Fact]
    public void CannotAddDuplicateTeams()
    {
        var manager = Manager.Create(new ManagerName("Jakob"), new Email("jakob@jakobsson.com"));

        manager.AddTeam(TestData.TeamEmpty("Laget"));

        Assert.Throws<ArgumentException>(() => manager.AddTeam(TestData.TeamEmpty("Laget")));
    }
}
