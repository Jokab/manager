using ManagerGame.Core;
using ManagerGame.Core.Drafting;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using ManagerGame.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test.Api;

public class DraftPickTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;

    public DraftPickTest(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task PickPlayerEnforcesOrderAndCompletesDraft()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var registerHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();
        var leagueHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateLeagueRequest, League>>();
        var teamHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTeamCommand, Team>>();
        var draftHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateDraftRequest, Draft>>();
        var startDraftHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<StartDraftRequest, Draft>>();
        var pickHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<PickDraftPlayerRequest, DraftPickOutcome>>();

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

        // Create 2 teams
        var teams = new List<Team>();
        for (var i = 0; i < 2; i++)
        {
            var teamResult = await teamHandler.Handle(new CreateTeamCommand
            {
                Name = new TeamName($"Lag-{i}-{Guid.NewGuid()}"),
                ManagerId = manager.Id,
                LeagueId = league.Id
            });
            Assert.True(teamResult.IsSuccess);
            teams.Add(teamResult.Value!);
        }

        var draftResult = await draftHandler.Handle(new CreateDraftRequest(league.Id));
        Assert.True(draftResult.IsSuccess);
        var draft = draftResult.Value!;

        var startResult = await startDraftHandler.Handle(new StartDraftRequest
        {
            DraftId = draft.Id,
            PicksPerTeam = 1
        });
        Assert.True(startResult.IsSuccess);

        // Seed players
        var player1 = TestData.Player();
        var player2 = TestData.Player(position: Position.Midfielder);
        db.Players.AddRange(player1, player2);
        await db.SaveChangesAsync();

        var firstTeamId = draft.Participants.First().TeamId;
        var secondTeamId = draft.Participants.ElementAt(1).TeamId;
        var firstTeam = teams.Single(t => t.Id == firstTeamId);
        var secondTeam = teams.Single(t => t.Id == secondTeamId);

        // First pick
        var pick1Result = await pickHandler.Handle(new PickDraftPlayerRequest
        {
            DraftId = draft.Id,
            TeamId = firstTeam.Id,
            PlayerId = player1.Id
        });

        Assert.True(pick1Result.IsSuccess);
        var pick1 = pick1Result.Value!;
        Assert.Equal(firstTeam.Id, pick1.Pick.TeamId);
        Assert.Equal(DraftState.Started, pick1.Draft.State);

        db.ChangeTracker.Clear();
        var updatedTeam = await db.Teams2.Find(firstTeam.Id);
        Assert.Single(updatedTeam!.Players);

        // Try to pick out of order (should fail)
        var badPickResult = await pickHandler.Handle(new PickDraftPlayerRequest
        {
            DraftId = draft.Id,
            TeamId = firstTeam.Id,
            PlayerId = player2.Id
        });
        Assert.True(badPickResult.IsFailure);

        // Second pick (completes draft)
        var pick2Result = await pickHandler.Handle(new PickDraftPlayerRequest
        {
            DraftId = draft.Id,
            TeamId = secondTeam.Id,
            PlayerId = player2.Id
        });

        Assert.True(pick2Result.IsSuccess);
        var pick2 = pick2Result.Value!;
        Assert.Equal(DraftState.Finished, pick2.Draft.State);
        Assert.Null(pick2.NextTeamId);

        // Try to pick after draft finished (should fail)
        var afterFinishedResult = await pickHandler.Handle(new PickDraftPlayerRequest
        {
            DraftId = draft.Id,
            TeamId = secondTeam.Id,
            PlayerId = Guid.NewGuid()
        });
        Assert.True(afterFinishedResult.IsFailure);
    }
}
