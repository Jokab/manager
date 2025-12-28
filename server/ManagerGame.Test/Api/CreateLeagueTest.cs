using ManagerGame.Core;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using ManagerGame.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test.Api;

public class CreateLeagueTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;

    public CreateLeagueTest(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Test()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var registerHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();
        var leagueHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateLeagueRequest, League>>();
        var teamHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTeamCommand, Team>>();

        // Register manager
        var email = $"jakob{Guid.NewGuid()}@jakobsson.com";
        var managerResult = await registerHandler.Handle(new RegisterManagerCommand
        {
            Name = new ManagerName("Jakob"),
            Email = new Email(email)
        });
        Assert.True(managerResult.IsSuccess);
        var manager = managerResult.Value!;

        // Create league
        var leagueResult = await leagueHandler.Handle(new CreateLeagueRequest { Name = "Test League" });
        Assert.True(leagueResult.IsSuccess);
        var league = leagueResult.Value!;

        // Create team
        var teamResult = await teamHandler.Handle(new CreateTeamCommand
        {
            Name = new TeamName($"Lag-{Guid.NewGuid()}"),
            ManagerId = manager.Id,
            LeagueId = league.Id
        });
        Assert.True(teamResult.IsSuccess);
        var team = teamResult.Value!;

        // Verify team is in the league
        db.ChangeTracker.Clear();
        var teamInDb = await db.Teams2.Find(team.Id);
        Assert.NotNull(teamInDb);
        Assert.Equal(league.Id, teamInDb.LeagueId);
    }
}
