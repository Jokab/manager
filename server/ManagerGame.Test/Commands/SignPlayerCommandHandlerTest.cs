using ManagerGame.Core;
using ManagerGame.Core.Teams;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Test.Commands;

public class SignPlayerCommandHandlerTest
{
    private static ApplicationDbContext CreateDb()
    {
        // Use SQLite in-memory, consistent with the rest of the test suite (and closer to production behavior).
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var db = new ApplicationDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    [Fact]
    public async Task PersistsPlayerInTeam()
    {
        await using var db = CreateDb();
        var manager = Manager.Create(new ManagerName("M1"), new Email("m1@test.se"));
        var team = Team.Create(new TeamName("Laget"), manager.Id, [], null);
        var player = TestData.Player();

        manager.AddTeam(team);
        db.Managers.Add(manager);
        db.Teams.Add(team);
        db.Players.Add(player);
        await db.SaveChangesAsync();

        Assert.Empty(team.Players);
        Assert.Null(player.TeamId);
        var sut = new SignPlayerCommandHandler(db);

        await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

        var teamPlayers = await db.TeamPlayers2.GetAll();
        Assert.Single(teamPlayers);
        var tp = teamPlayers.Single();
        Assert.Equal(player.Id, tp.Player.Id);
        Assert.Equal(team.Id, tp.TeamId);
        Assert.Equal("Jakob", tp.Player.Name.Name);
        Assert.Equal(Position.Defender, tp.Player.Position);
        Assert.Equal(Country.Se, tp.Player.Country.Country);
    }

    [Fact]
    public async Task CannotSignSignedPlayer()
    {
        await using var db = CreateDb();
        var manager = Manager.Create(new ManagerName("M1"), new Email("m1@test.se"));
        var team = Team.Create(new TeamName("Laget"), manager.Id, [], null);
        var team2 = Team.Create(new TeamName("Laget2"), manager.Id, [], null);
        var player = TestData.Player();
        manager.AddTeam(team);
        manager.AddTeam(team2);
        db.Managers.Add(manager);
        db.Teams.AddRange(team, team2);
        db.Players.Add(player);
        await db.SaveChangesAsync();

        Assert.Empty(team.Players);
        Assert.Empty(team2.Players);
        var sut = new SignPlayerCommandHandler(db);
        await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

        Assert.True(player.IsSigned);

        var signResult = await sut.Handle(new SignPlayerRequest(team2.Id, player.Id));

        Assert.True(signResult.IsFailure);
        Assert.NotEmpty(signResult.Error.Code);
    }
}
