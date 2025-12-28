using ManagerGame.Core;
using ManagerGame.Core.Drafting;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using ManagerGame.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test.Api;

public class CreateDraftTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;

    public CreateDraftTest(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Test()
    {
        using var scope = _fixture.Services.CreateScope();
        var registerHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();
        var leagueHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateLeagueRequest, League>>();
        var teamHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTeamCommand, Team>>();
        var draftHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateDraftRequest, Draft>>();

        var email = $"jakob{Guid.NewGuid()}@jakobsson.com";
        var managerResult = await registerHandler.Handle(new RegisterManagerCommand
        {
            Name = new ManagerName("Jakob"),
            Email = new Email(email)
        });
        Assert.True(managerResult.IsSuccess);
        var manager = managerResult.Value!;

        var leagueResult = await leagueHandler.Handle(new CreateLeagueRequest { Name = "Test League" });
        Assert.True(leagueResult.IsSuccess);
        var league = leagueResult.Value!;

        // Create 4 teams
        for (var i = 0; i < 4; i++)
        {
            var teamResult = await teamHandler.Handle(new CreateTeamCommand
            {
                Name = new TeamName($"Lag-{i}-{Guid.NewGuid()}"),
                ManagerId = manager.Id,
                LeagueId = league.Id
            });
            Assert.True(teamResult.IsSuccess);
        }

        var draftResult = await draftHandler.Handle(new CreateDraftRequest(league.Id));
        Assert.True(draftResult.IsSuccess);
        Assert.Equal(DraftState.Created, draftResult.Value!.State);
    }
}
