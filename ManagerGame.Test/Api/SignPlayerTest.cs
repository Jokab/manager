using System.Net;
using System.Net.Http.Headers;
using ManagerGame.Api;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Test.Api;

public class SignPlayerTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public SignPlayerTest(Fixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
    }

    [Fact]
    public async Task SignFirstPlayer()
    {
        var (manager, newTeam) = await SeedAndLogin();
        var db = TestDbFactory.Create(_webApplicationFactory.Services);
        var player = new Player(Guid.NewGuid(), new PlayerName("Jakob"), Position.Defender);
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var (httpResponseMessage, signPlayerDto) =
	        await _httpClient.Post<SignPlayerDto>("/api/teams/sign", new SignPlayerRequest(newTeam.Id, player.Id));

        var (_, team) = await _httpClient.Get<Team>($"/api/teams/{newTeam.Id}");

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Single(team!.Players);

        Assert.Equal("Jakob", team.Players.First().Name.Name);
        Assert.Equal(Position.Defender, team.Players.First().Position);

        Assert.Equal("Jakob", db.Teams.Include(t => t.Players).First().Players.First().Name.Name);
        Assert.Equal(Position.Defender, db.Teams.Include(t => t.Players).First().Players.First().Position);
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

