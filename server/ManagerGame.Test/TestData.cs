using ManagerGame.Domain;

namespace ManagerGame.Test;

public static class TestData
{
    public static Player Player(Country country = Country.Se,
        Position position = Position.Defender)
    {
        var player = new Player(
            new PlayerName("Jakob"),
            position,
            new CountryRec(country));
        // Set ID for domain tests (since EF Core won't generate them)
        SetId(player, Guid.NewGuid());
        return player;
    }

    public static Team TeamEmpty(string name)
    {
        var league = League.Empty();
        SetId(league, Guid.NewGuid());
        var team = Team.Create(new TeamName(name), Guid.NewGuid(), [], league.Id);
        SetId(team, Guid.NewGuid());
        return team;
    }

    public static Team TeamWithValidFullSquad(string name = "Lag")
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

        var league = League.Empty();
        SetId(league, Guid.NewGuid());
        var team = Team.Create(new TeamName(name),
            Guid.NewGuid(),
            players,
            league.Id);
        SetId(team, Guid.NewGuid());
        return team;
    }

    // Helper method to set ID for domain tests (since EF Core won't generate them)
    private static void SetId(Entity entity, Guid id)
    {
        entity.Id = id;
    }
}
