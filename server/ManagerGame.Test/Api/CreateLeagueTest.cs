using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Leagues;

namespace ManagerGame.Test.Api;

public class CreateLeagueTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;

    public CreateLeagueTest(Fixture fixture)
    {
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task Test()
    {
        (_, TeamDto? team) = await Seed.SeedAndLogin(_httpClient);
        (HttpResponseMessage? createHttp, CreateLeagueDto? createLeagueDto) =
            await _httpClient.Post<CreateLeagueDto>("/api/leagues", new CreateLeagueRequest());

        Assert.Equal(HttpStatusCode.OK, createHttp.StatusCode);

        (HttpResponseMessage? admit1Http, AdmitTeamDto? admit1) = await _httpClient.Post<AdmitTeamDto>("/api/leagues/admitTeam",
            new AdmitTeamRequest { LeagueId = createLeagueDto!.Id, TeamId = team.Id });

        Assert.Equal(HttpStatusCode.OK, admit1Http.StatusCode);
        Assert.Contains(admit1!.League.Teams, x => x.Id == team.Id);

        (_, TeamDto? team1) = await _httpClient.Get<TeamDto>($"/api/teams/{team.Id}");
        Assert.Equal(createLeagueDto.Id, team1!.League!.Id);
    }
}
