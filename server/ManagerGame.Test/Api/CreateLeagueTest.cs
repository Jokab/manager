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
        var (_, leagueId, team) = await Seed.SeedAndLogin(_httpClient);
        var (_, team1) = await _httpClient.Get<TeamDto>($"/api/teams/{team.Id}");
        Assert.Equal(leagueId, team1!.LeagueId);
    }
}
