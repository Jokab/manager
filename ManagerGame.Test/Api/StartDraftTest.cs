using System.Net;
using ManagerGame.Api.Drafting;
using ManagerGame.Api.Leagues;

namespace ManagerGame.Test.Api;

public class StartDraftTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;

    public StartDraftTest(Fixture fixture)
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

        var (http, createDraftDto) =
            await _httpClient.Post<CreateDraftDto>("/api/drafts", new CreateDraftRequest(createLeagueDto.Id));

        Assert.Equal(HttpStatusCode.OK, http.StatusCode);
        Assert.NotNull(createDraftDto);
    }
}
