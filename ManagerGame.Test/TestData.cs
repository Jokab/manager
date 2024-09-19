using ManagerGame.Core.Domain;

namespace ManagerGame.Test;

public static class TestData
{
    public static Player Player() =>
        new(Guid.NewGuid(),
            new PlayerName("Jakob"),
            Position.Defender,
            new CountryRec(Country.Se));
}
