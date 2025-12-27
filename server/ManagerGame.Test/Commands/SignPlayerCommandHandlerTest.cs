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
        var league = League.Empty();
        var manager = Manager.Create(new ManagerName("M1"), new Email("m1@test.se"));
        var player = TestData.Player();

        db.Leagues.Add(league);
        db.Managers.Add(manager);
        db.Players.Add(player);
        await db.SaveChangesAsync(); // Save to get IDs

        var team = Team.Create(new TeamName("Laget"), manager.Id, [], league.Id);
        manager.AddTeam(team);
        db.Teams.Add(team);
        await db.SaveChangesAsync();

        Assert.Empty(team.Players);
        var sut = CreateHandler(db);

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
    public async Task CannotSignDuplicatePlayerInSameTeam()
    {
        await using var db = CreateDb();
        var league = League.Empty();
        var manager = Manager.Create(new ManagerName("M1"), new Email("m1@test.se"));
        var player = TestData.Player();

        db.Leagues.Add(league);
        db.Managers.Add(manager);
        db.Players.Add(player);
        await db.SaveChangesAsync(); // Save to get IDs

        var team = Team.Create(new TeamName("Laget"), manager.Id, [], league.Id);
        manager.AddTeam(team);
        db.Teams.Add(team);
        await db.SaveChangesAsync();

        Assert.Empty(team.Players);
        var sut = CreateHandler(db);
        await sut.Handle(new SignPlayerRequest(team.Id, player.Id));
        var signResult = await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

        Assert.True(signResult.IsFailure);
        Assert.NotEmpty(signResult.Error.Code);
    }

    [Fact]
    public async Task CanSignSamePlayerInDifferentLeagues()
    {
        await using var db = CreateDb();
        var league1 = League.Empty();
        var league2 = League.Empty();
        var manager = Manager.Create(new ManagerName("M1"), new Email("m1@test.se"));
        var player = TestData.Player();

        db.Leagues.AddRange(league1, league2);
        db.Managers.Add(manager);
        db.Players.Add(player);
        await db.SaveChangesAsync(); // Save to get IDs

        var team1 = Team.Create(new TeamName("Laget1"), manager.Id, [], league1.Id);
        var team2 = Team.Create(new TeamName("Laget2"), manager.Id, [], league2.Id);
        manager.AddTeam(team1);
        manager.AddTeam(team2);
        db.Teams.AddRange(team1, team2);
        await db.SaveChangesAsync();

        var sut = CreateHandler(db);
        var r1 = await sut.Handle(new SignPlayerRequest(team1.Id, player.Id));
        var r2 = await sut.Handle(new SignPlayerRequest(team2.Id, player.Id));

        Assert.True(r1.IsSuccess);
        Assert.True(r2.IsSuccess);
    }

    [Fact]
    public async Task CannotSignSamePlayerInSameLeagueDifferentTeams()
    {
        await using var db = CreateDb();
        var league = League.Empty();
        var manager = Manager.Create(new ManagerName("M1"), new Email("m1@test.se"));
        var player = TestData.Player();

        db.Leagues.Add(league);
        db.Managers.Add(manager);
        db.Players.Add(player);
        await db.SaveChangesAsync(); // Save to get IDs

        var team1 = Team.Create(new TeamName("Laget1"), manager.Id, [], league.Id);
        var team2 = Team.Create(new TeamName("Laget2"), manager.Id, [], league.Id);
        manager.AddTeam(team1);
        manager.AddTeam(team2);
        db.Teams.AddRange(team1, team2);
        await db.SaveChangesAsync();

        var sut = CreateHandler(db);
        var r1 = await sut.Handle(new SignPlayerRequest(team1.Id, player.Id));
        var r2 = await sut.Handle(new SignPlayerRequest(team2.Id, player.Id));

        Assert.True(r1.IsSuccess);
        Assert.True(r2.IsFailure);
        Assert.Equal("Player already drafted in this league", r2.Error.Code);
    }

    private static SignPlayerCommandHandler CreateHandler(ApplicationDbContext db) =>
        new(new TeamSigningService(db));
}
