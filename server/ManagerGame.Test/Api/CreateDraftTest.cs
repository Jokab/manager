using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Api.Requests;
using ManagerGame.Core.Drafting;
using ManagerGame.Core.Leagues;

namespace ManagerGame.Test.Api;

public class CreateDraftTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;

    public CreateDraftTest(Fixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task Test()
    {
        TestDbFactory.Create(_fixture);

        (var manager, _) = await Seed.SeedManagerAndLogin(_httpClient);

        var teams = new List<TeamDto>();
        for (var i = 0; i < 4; i++)
        {
            (_, var t) = await _httpClient.Post<TeamDto>("/api/teams",
                new CreateTeamRequest { Name = $"Lag-{i}-{Guid.NewGuid()}", ManagerId = manager.Id });
            teams.Add(t!);
        }
        (_, var createLeagueDto) =
            await _httpClient.Post<CreateLeagueDto>("/api/leagues", new CreateLeagueRequest());
        Assert.NotNull(createLeagueDto);
        var leagueId = createLeagueDto.Id;

        foreach (var t in teams)
        {
            await _httpClient.Post<CreateLeagueDto>("/api/leagues/admitTeam",
                new AdmitTeamRequest { LeagueId = leagueId, TeamId = t.Id });
        }

        (var http1, var createDraftDto) =
            await _httpClient.Post<CreateDraftDto>("/api/drafts", new CreateDraftRequest(leagueId));

        Assert.Equal(HttpStatusCode.OK, http1.StatusCode);
        Assert.Equal(DraftState.Created, createDraftDto!.State);

        (var http2, var startDraftDto) =
            await _httpClient.Post<StartDraftDto>("/api/drafts/start",
                new StartDraftRequest { DraftId = createDraftDto.Id });

        Assert.Equal(HttpStatusCode.OK, http2.StatusCode);
        Assert.Equal(DraftState.Started, startDraftDto!.State);
    }
}
