using ManagerGame.Core.Domain;

namespace ManagerGame.Test;

public static class TestData
{
    public static Player Player(Country country = Country.Se,
        Position position = Position.Defender) =>
        new(Guid.NewGuid(),
            new PlayerName("Jakob"),
            position,
            new CountryRec(country));

    public static Team TeamEmpty(string name) =>
        Team.Create(new TeamName(name), Guid.NewGuid(), [], League.Empty());

    public static Team TeamWithValidFullSquad()
    {
        const int countriesToChooseFrom = Team.PlayerLimit / Team.PlayersFromSameCountryLimit;
        var goalkeepersRemaining = 1;
        var minDefendersRemaining = 4;
        var minMidfieldersRemaining = 4;
        var players = Enumerable.Range(0, countriesToChooseFrom).SelectMany(i =>
            Enumerable.Range(0, Team.PlayersFromSameCountryLimit).Select(_ =>
            {
                Position position;
                if (goalkeepersRemaining > 0)
                {
                    goalkeepersRemaining--;
                    position = Position.Goalkeeper;
                }
                else if (minDefendersRemaining > 0)
                {
                    minDefendersRemaining--;
                    position = Position.Defender;
                }
                else if (minMidfieldersRemaining > 0)
                {
                    minMidfieldersRemaining--;
                    position = Position.Midfielder;
                }
                else
                {
                    position = Position.Forward;
                }

                return Player((Country)i, position);
            })
        ).ToList();
        // TODO: Clean this up
        players.Add(Player(Country.Es));
        players.Add(Player(Country.Es));

        return Team.Create(new TeamName("Lag"),
            Guid.NewGuid(),
            players,
            League.Empty());
    }
}
