using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Api.Requests;
using ManagerGame.Core;
using ManagerGame.Core.Drafting;
using ManagerGame.Core.Leagues;

namespace ManagerGame.Test.Api;

public class DraftPickTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;

    public DraftPickTest(Fixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task PickPlayerEnforcesOrderAndCompletesDraft()
    {
        var db = TestDbFactory.Create(_fixture);
        var (manager, _) = await Seed.SeedManagerAndLogin(_httpClient);
        var leagueId = await CreateLeagueAsync();
        var teams = await CreateTeamsAsync(leagueId, manager.Id, 2);
        var draftId = await CreateDraftAsync(leagueId);
        await StartDraftAsync(draftId, picksPerTeam: 1);
        var draftDetails = await GetDraftAsync(draftId);

        var (player1, player2) = await SeedPlayersAsync(db);

        var firstTeamId = draftDetails.ParticipantTeamIds.First();
        var secondTeamId = draftDetails.ParticipantTeamIds.ElementAt(1);
        var firstTeam = teams.Single(t => t.Id == firstTeamId);
        var secondTeam = teams.Single(t => t.Id == secondTeamId);

        var (pickResponse1, pick1) = await _httpClient.Post<DraftPickResultDto>("/api/drafts/pick",
            new PickDraftPlayerRequest { DraftId = draftId, TeamId = firstTeam.Id, PlayerId = player1.Id });

        Assert.Equal(HttpStatusCode.OK, pickResponse1.StatusCode);
        Assert.NotNull(pick1);
        Assert.Equal(firstTeam.Id, pick1!.Pick.TeamId);
        Assert.Equal(DraftState.Started, pick1.DraftState);

        var (_, updatedTeam) = await _httpClient.Get<TeamDto>($"/api/teams/{firstTeam.Id}");
        Assert.Single(updatedTeam!.Players);

        var (badPickResponse, _) = await _httpClient.Post<DraftPickResultDto>("/api/drafts/pick",
            new PickDraftPlayerRequest { DraftId = draftId, TeamId = firstTeam.Id, PlayerId = player2.Id });
        Assert.Equal(HttpStatusCode.InternalServerError, badPickResponse.StatusCode);

        var (pickResponse2, pick2) = await _httpClient.Post<DraftPickResultDto>("/api/drafts/pick",
            new PickDraftPlayerRequest { DraftId = draftId, TeamId = secondTeam.Id, PlayerId = player2.Id });

        Assert.Equal(HttpStatusCode.OK, pickResponse2.StatusCode);
        Assert.NotNull(pick2);
        Assert.Equal(DraftState.Finished, pick2!.DraftState);
        Assert.Null(pick2.NextTeamId);

        var (afterFinishedResponse, _) = await _httpClient.Post<DraftPickResultDto>("/api/drafts/pick",
            new PickDraftPlayerRequest { DraftId = draftId, TeamId = secondTeam.Id, PlayerId = Guid.NewGuid() });
        Assert.Equal(HttpStatusCode.InternalServerError, afterFinishedResponse.StatusCode);
    }
    private async Task<Guid> CreateLeagueAsync()
    {
        var (_, createLeagueDto) =
            await _httpClient.Post<CreateLeagueDto>("/api/leagues", new CreateLeagueRequest { Name = "Test League" });
        Assert.NotNull(createLeagueDto);
        return createLeagueDto.Id;
    }

    private async Task<List<TeamDto>> CreateTeamsAsync(Guid leagueId, Guid managerId, int count)
    {
        var teams = new List<TeamDto>();
        for (var i = 0; i < count; i++)
        {
            var (_, team) = await _httpClient.Post<TeamDto>("/api/teams",
                new CreateTeamRequest { Name = $"Lag-{i}-{Guid.NewGuid()}", ManagerId = managerId, LeagueId = leagueId });
            teams.Add(team!);
        }

        return teams;
    }

    private async Task<Guid> CreateDraftAsync(Guid leagueId)
    {
        var (_, draftDto) =
            await _httpClient.Post<CreateDraftDto>("/api/drafts", new CreateDraftRequest(leagueId));
        Assert.NotNull(draftDto);
        return draftDto.Id;
    }

    private async Task<DraftDto> GetDraftAsync(Guid draftId)
    {
        var (_, draftDto) = await _httpClient.Get<DraftDto>($"/api/drafts/{draftId}");
        return draftDto!;
    }

    private async Task StartDraftAsync(Guid draftId, int picksPerTeam)
    {
        var (response, dto) = await _httpClient.Post<StartDraftDto>("/api/drafts/start",
            new StartDraftRequest { DraftId = draftId, PicksPerTeam = picksPerTeam });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(dto);
        Assert.Equal(DraftState.Started, dto!.State);
    }

    private static async Task<(Player player1, Player player2)> SeedPlayersAsync(ApplicationDbContext db)
    {
        var player1 = TestData.Player();
        var player2 = TestData.Player(position: Position.Midfielder);
        db.Players.AddRange(player1, player2);
        await db.SaveChangesAsync();
        return (player1, player2);
    }
}


