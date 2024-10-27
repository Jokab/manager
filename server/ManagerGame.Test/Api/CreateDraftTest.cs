using System.Net;
using ManagerGame.Api.Dtos;
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

        var (_, team) = await Seed.SeedAndLogin(_httpClient);
        var (_, createLeagueDto) =
            await _httpClient.Post<CreateLeagueDto>("/api/leagues", new CreateLeagueRequest());

        await _httpClient.Post<CreateLeagueDto>("/api/leagues/admitTeam",
            new AdmitTeamRequest { LeagueId = createLeagueDto!.Id, TeamId = team.Id });
        await _httpClient.Post<CreateLeagueDto>("/api/leagues/admitTeam",
            new AdmitTeamRequest { LeagueId = createLeagueDto.Id, TeamId = team.Id });

        var (http1, createDraftDto) =
            await _httpClient.Post<CreateDraftDto>("/api/drafts", new CreateDraftRequest(createLeagueDto.Id));

        Assert.Equal(HttpStatusCode.OK, http1.StatusCode);
        Assert.Equal(DraftState.Created, createDraftDto!.State);

        var (http2, startDraftDto) =
            await _httpClient.Post<StartDraftDto>("/api/drafts/start",
                new StartDraftRequest { DraftId = createDraftDto.Id });

        Assert.Equal(HttpStatusCode.OK, http2.StatusCode);
        Assert.Equal(DraftState.Started, startDraftDto!.State);
    }
}
