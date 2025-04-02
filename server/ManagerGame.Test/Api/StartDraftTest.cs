using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Drafting;
using ManagerGame.Core.Leagues;

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
        TestDbFactory.Create(_fixture);

        (_, TeamDto? team) = await Seed.SeedAndLogin(_httpClient);
        (_, CreateLeagueDto? createLeagueDto) =
            await _httpClient.Post<CreateLeagueDto>("/api/leagues", new CreateLeagueRequest());

        await _httpClient.Post<CreateLeagueDto>("/api/leagues/admitTeam",
            new AdmitTeamRequest { LeagueId = createLeagueDto!.Id, TeamId = team.Id });
        await _httpClient.Post<CreateLeagueDto>("/api/leagues/admitTeam",
            new AdmitTeamRequest { LeagueId = createLeagueDto.Id, TeamId = team.Id });

        (HttpResponseMessage? http, CreateDraftDto? createDraftDto) =
            await _httpClient.Post<CreateDraftDto>("/api/drafts", new CreateDraftRequest(createLeagueDto.Id));

        Assert.Equal(HttpStatusCode.OK, http.StatusCode);
        Assert.NotNull(createDraftDto);
    }
}
