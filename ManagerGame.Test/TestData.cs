using ManagerGame.Core.Domain;

namespace ManagerGame.Test;

public static class TestData
{
    public static Player Player(Country country = Country.Se) =>
        new(Guid.NewGuid(),
            new PlayerName("Jakob"),
            Position.Defender,
            new CountryRec(country));
}
