using System.Net;
using System.Net.Http.Headers;
using ManagerGame.Api;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Api;

public class SignPlayerTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;


    public SignPlayerTest(Fixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task SignFirstPlayer()
    {
        var db = TestDbFactory.Create(_fixture);

        var (manager, newTeam) = await SeedAndLogin();

        var player = TestData.Player();
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var (httpResponseMessage, signPlayerDto) =
            await _httpClient.Post<SignPlayerDto>("/api/teams/sign", new SignPlayerRequest(newTeam.Id, player.Id));

        var (_, team) = await _httpClient.Get<Team>($"/api/teams/{newTeam.Id}");

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Single(team!.Players);

        Assert.Equal("Jakob", team.Players.First().Name.Name);
        Assert.Equal(Position.Defender, team.Players.First().Position);
        Assert.Equal(team.Id, team.Players.First().TeamId);
        Assert.Equal(1000, team.Players.First().MarketValue.Value);
        Assert.Equal(Country.Se, team.Players.First().Country.Country);
    }

    private async Task<(ManagerDto manager, TeamDto team)> SeedAndLogin()
    {
        var (_, manager) = await _httpClient.PostManager<ManagerDto>();
        var createTeamRequest = new CreateTeamRequest
            { Name = new TeamName("Lag"), ManagerId = manager!.Id };
        var (_, login) =
            await _httpClient.Post<LoginResponseDto>("/api/login", new LoginRequest { ManagerId = manager.Id });

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        var (_, team) = await _httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        return (manager, team!);
    }
}
