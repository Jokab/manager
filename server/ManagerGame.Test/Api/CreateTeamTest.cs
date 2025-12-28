using ManagerGame.Core;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using ManagerGame.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test.Api;

public class CreateTeamTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;

    public CreateTeamTest(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateTeam()
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
            Name = new TeamName("Lag2"),
            ManagerId = manager.Id,
            LeagueId = league.Id
        });

        Assert.True(teamResult.IsSuccess);
        var team = teamResult.Value!;
        Assert.Equal(manager.Id, team.ManagerId);
        Assert.Equal("Lag2", team.Name.Name);

        db.ChangeTracker.Clear();
        var allTeams = await db.Teams2.GetAll();
        Assert.Single(allTeams);
        var createdManagerInDb = await db.Managers
            .Include(m => m.Teams)
            .FirstAsync(x => x.Id == manager.Id);
        var createdTeamInDb = createdManagerInDb.Teams.First(x => x.Id == team.Id);
        Assert.Equal(manager.Id, createdTeamInDb.ManagerId);
        Assert.Equal("Lag2", createdTeamInDb.Name.Name);
    }
}
