using System.Net;
using ManagerGame.Api.Drafting;
using ManagerGame.Api.Leagues;
using ManagerGame.Core.Domain;

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
        var db = TestDbFactory.Create(_fixture);

        var (manager, team) = await Seed.SeedAndLogin(_httpClient);
        var (httpResponseMessage, createLeagueDto) =
            await _httpClient.Post<CreateLeagueDto>("/api/leagues", new CreateLeagueRequest());

        var (_, _) = await _httpClient.Post<CreateLeagueDto>("/api/leagues/admitTeam",
            new AdmitTeamRequest { LeagueId = createLeagueDto!.Id, TeamId = team.Id });
        var (_, _) = await _httpClient.Post<CreateLeagueDto>("/api/leagues/admitTeam",
            new AdmitTeamRequest { LeagueId = createLeagueDto.Id, TeamId = team.Id });

        var (http1, createDraftDto) =
            await _httpClient.Post<CreateDraftDto>("/api/drafts", new CreateDraftRequest(createLeagueDto.Id));

        Assert.Equal(HttpStatusCode.OK, http1.StatusCode);
        Assert.Equal(State.Created, createDraftDto!.State);

        var (http2, startDraftDto) =
            await _httpClient.Post<StartDraftDto>("/api/drafts/start",
                new StartDraftRequest { DraftId = createDraftDto.Id });

        Assert.Equal(HttpStatusCode.OK, http2.StatusCode);
        Assert.Equal(State.Started, startDraftDto!.State);
    }
}
