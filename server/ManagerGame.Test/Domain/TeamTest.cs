namespace ManagerGame.Test.Domain;

public class TeamTest
{
    [Fact]
    public void CanSignNewPlayer()
    {
        Team team = TestData.TeamEmpty("Laget");
        Player player = TestData.Player();
        Assert.Empty(team.Players);

        team.SignPlayer(player);

        Assert.Contains(player, team.Players.Select(x => x.Player));
    }

    [Fact]
    public void CannotSignDuplicatePlayer()
    {
        Team team = TestData.TeamEmpty("Laget");
        Player player = TestData.Player();
        Assert.Empty(team.Players);

        team.SignPlayer(player);

        Assert.Throws<ArgumentException>(() => team.SignPlayer(player));
    }

    [Theory]
    [InlineData(Country.Se, 0)]
    [InlineData(Country.Dk, 0)]
    public void CannotSignMoreThanLimitFromSameCountry(Country country,
        int initialPlayersCountOverLimit)
    {
        List<Player> players = Enumerable.Range(0, Team.PlayersFromSameCountryLimit + initialPlayersCountOverLimit)
            .Select(_ => TestData.Player(country)).ToList();
        var team = Team.Create(new TeamName("Lag"),
            Guid.NewGuid(),
            players,
            League.Empty());

        Player newPlayer = TestData.Player(country);

        Assert.Throws<ArgumentException>(() => team.SignPlayer(newPlayer));
    }

    [Theory]
    [InlineData(Country.Se, 1)]
    [InlineData(Country.Se, 2)]
    [InlineData(Country.Dk, 1)]
    public void CanSignFromSameCountryUpToLimit(Country country,
        int initialPlayersCountBelowLimit)
    {
        List<Player> players = Enumerable.Range(0, Team.PlayersFromSameCountryLimit - initialPlayersCountBelowLimit)
            .Select(_ => TestData.Player(country)).ToList();
        var team = Team.Create(new TeamName("Lag"),
            Guid.NewGuid(),
            players,
            League.Empty());

        Player newPlayer = TestData.Player(country);
        team.SignPlayer(newPlayer);

        Assert.Contains(newPlayer, team.Players.Select(x => x.Player));
    }

    [Fact]
    public void CannotSignMoreThanPlayerLimit()
    {
        Team team = TestData.TeamWithValidFullSquad();

        Assert.Throws<ArgumentException>(() => team.SignPlayer(TestData.Player()));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void CanSignPlayersBelowLimit(int initialPlayersCountBelowLimit)
    {
        Team team = TestData.TeamWithValidFullSquad();
        for (var i = 0; i < initialPlayersCountBelowLimit; i++)
            // Remove from full squad so we can add them in test
            team.Players.Remove(team.Players.First(x => x.Player.Country.Country == Country.Se));
        var newPlayer = TestData.Player();

        team.SignPlayer(newPlayer);

        Assert.Contains(newPlayer, team.Players.Select(x => x.Player));
    }

    [Fact]
    public void CannotSignSignedPlayer()
    {
        var team = TestData.TeamEmpty("Team");

        var signedPlayer = TestData.Player(position: Position.Midfielder);
        signedPlayer.TeamId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => team.SignPlayer(signedPlayer));
    }

    [Fact]
    public void CannotSignIfCannotConformToFormation()
    {
        Team team = TestData.TeamWithValidFullSquad();

        team.Players.Remove(team.Players.First(x => x.Player.Position == Position.Midfielder));

        Assert.Throws<ArgumentException>(() => team.SignPlayer(TestData.Player(position: Position.Midfielder)));
    }

    [Fact]
    public void CanSignIfConformsToFormation()
    {
        // TODO:
    }
}
