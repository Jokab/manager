using ManagerGame.Core.Domain;

namespace ManagerGame.Test;

public static class TestData
{
    public static Player Player()
    {
        return new Player(Guid.NewGuid(),
            new PlayerName("Jakob"),
            Position.Defender,
            new MarketValue(1000),
            new CountryRec(Country.Se));
    }
}
