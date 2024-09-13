using System.Net;
using System.Net.Http.Headers;
using ManagerGame.Api;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ManagerGame.Test.Api;

public class AddPlayerTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public AddPlayerTest(Fixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
    }

    [Fact]
    public async Task SignPlayer()
    {
        var (manager, newTeam) = await SeedAndLogin();

        var (httpResponseMessage, signPlayerDto) = await _httpClient.Post<SignPlayerDto>("/api/teams/sign", new SignPlayerRequest(newTeam.Id));

        var (_, team) = await _httpClient.Get<Team>($"/api/teams/{newTeam.Id}");

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Single(team!.Players);
        var playerInTeam = team.Players.First();
        Assert.Equal("Jakob", playerInTeam.Name.Name);
        Assert.Equal(Position.Defender, playerInTeam.Position);
    }

    private async Task<(ManagerDto manager, TeamDto team)> SeedAndLogin()
    {
        var (_, manager) = await _httpClient.PostManager<ManagerDto>();
        var createTeamRequest = new CreateTeamRequest
            { Name = new TeamName("Lag"), ManagerId = manager!.Id };
        var db = TestDbFactory.Create(_webApplicationFactory.Services);
        var (_, login) =
            await _httpClient.Post<LoginResponseDto>("/api/login", new LoginRequest { ManagerId = manager.Id });

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        var (_, team) = await _httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        return (manager, team!);
    }
}

