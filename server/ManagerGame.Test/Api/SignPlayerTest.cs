using ManagerGame.Core;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using ManagerGame.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test.Api;

public class SignPlayerTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;

    public SignPlayerTest(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SignFirstPlayer()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var registerHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();
        var leagueHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateLeagueRequest, League>>();
        var teamHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTeamCommand, Team>>();
        var signHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<SignPlayerRequest, Team>>();

        var (manager, leagueId, team) = await Seed.SeedAndLogin(_fixture.Services);

        var player = TestData.Player();
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var signResult = await signHandler.Handle(new SignPlayerRequest(team.Id, player.Id));
        Assert.True(signResult.IsSuccess);

        db.ChangeTracker.Clear();
        var updatedTeam = await db.Teams2.Find(team.Id);
        Assert.NotNull(updatedTeam);

        Assert.Single(updatedTeam.Players);
        Assert.Equal("Jakob", updatedTeam.Players.First().Player.Name.Name);
        Assert.Equal(Position.Defender, updatedTeam.Players.First().Player.Position);
        Assert.Equal(Country.Se, updatedTeam.Players.First().Player.Country.Country);
    }
}
