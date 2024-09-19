using ManagerGame.Core.Domain;

namespace ManagerGame.Test;

public static class TestData
{
    public static Player Player(Country country = Country.Se)
    {
        return new Player(Guid.NewGuid(),
            new PlayerName("Jakob"),
            Position.Defender,
            new CountryRec(country));
    }

    public static Team TeamWithValidFullSquad()
    {
        return Team.Create(new TeamName("Lag"),
            Guid.NewGuid(),
            Enumerable.Range(0, Team.PlayerLimit / Team.PlayersFromSameCountryLimit).SelectMany(i =>
                Enumerable.Range(0, Team.PlayersFromSameCountryLimit).Select(_ => Player((Country)i))
            ).ToList());
    }
}
