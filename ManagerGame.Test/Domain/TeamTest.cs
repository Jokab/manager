using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Domain;

public class TeamTest
{
    [Fact]
    public void CanSignNewPlayer()
    {
        var team = Team.Create(new TeamName("Lag"), Guid.NewGuid(), []);
        var player = TestData.Player();
        Assert.Empty(team.Players);

        team.SignPlayer(player);

        Assert.Contains(player, team.Players);
    }
    
    [Fact]
    public void CannotSignDuplicatePlayer()
    {
        var team = Team.Create(new TeamName("Lag"), Guid.NewGuid(), []);
        var player = TestData.Player();
        Assert.Empty(team.Players);

        team.SignPlayer(player);

        Assert.Throws<ArgumentException>(() => team.SignPlayer(player));
    }
    
    [Theory]
    [InlineData(Country.Se, 0)]
    [InlineData(Country.Se, 1)]
    [InlineData(Country.Dk, 0)]
    public void CannotSignMoreThanLimitFromSameCountry(Country country, int initialPlayersCountOverLimit)
    {
        var team = Team.Create(new TeamName("Lag"),
            Guid.NewGuid(),
            Enumerable.Range(0, Team.PlayersFromSameCountryLimit + initialPlayersCountOverLimit)
                .Select(_ => TestData.Player(country)).ToList());
        
        var newPlayer = TestData.Player(country);

        Assert.Throws<ArgumentException>(() => team.SignPlayer(newPlayer));
    }
    
    [Theory]
    [InlineData(Country.Se, 1)]
    [InlineData(Country.Se, 2)]
    [InlineData(Country.Se, 3)]
    [InlineData(Country.Se, 4)]
    [InlineData(Country.Dk, 4)]
    public void CanSignFromSameCountryUpToLimit(Country country, int initialPlayersCountBelowLimit)
    {
        var team = Team.Create(new TeamName("Lag"),
            Guid.NewGuid(),
            Enumerable.Range(0, Team.PlayersFromSameCountryLimit - initialPlayersCountBelowLimit)
                .Select(_ => TestData.Player(country)).ToList());

        for (var i = 0; i < initialPlayersCountBelowLimit; i++)
        {
            var newPlayer = TestData.Player(country);
            team.SignPlayer(newPlayer);
        }
    }
}
