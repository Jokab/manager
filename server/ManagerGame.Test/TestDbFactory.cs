using ManagerGame.Core;
using ManagerGame.Test.Api;

namespace ManagerGame.Test;

public static class TestDbFactory
{
    public static ApplicationDbContext Create(Fixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        var db = fixture.CreateContext();
        return db;
    }
}
