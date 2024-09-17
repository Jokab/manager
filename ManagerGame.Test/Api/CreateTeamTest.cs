using System.Net;
using System.Net.Http.Headers;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Test.Api;

public class CreateTeamTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;
    private readonly Fixture _fixture;

    public CreateTeamTest(Fixture fixture)
    {

        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task CreateTeam()
    {
        var db = TestDbFactory.Create(_fixture);

        var (createManagerResponse, manager) = await _httpClient.PostManager<ManagerDto>();
        var createTeamRequest = new CreateTeamRequest
            { Name = new TeamName("Lag"), ManagerId = manager!.Id };

        var (_, login) =
            await _httpClient.Post<LoginResponseDto>("/api/login", new LoginRequest { ManagerId = manager.Id });

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        var (createTeamResponse, team) = await _httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        Assert.Equal(HttpStatusCode.OK, createTeamResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);

        Assert.Equal(manager.Id, team!.ManagerId);
        Assert.Equal("Lag", team.Name.Name);

        Assert.Single(db.Teams);
        var createdManagerInDb = db.Managers.Include(m => m.Teams).First(x => x.Id == manager.Id);
        var createdTeamInDb = createdManagerInDb.Teams.First(x => x.Id == team.Id);
        Assert.Equal(manager.Id, createdTeamInDb.ManagerId);
        Assert.Equal("Lag", createdTeamInDb.Name.Name);
    }
}

public class UnauthorizedTeamTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;
    private readonly Fixture _fixture;

    public UnauthorizedTeamTest(Fixture fixture)
    {

        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task UnauthorizedIfNotLoggedIn()
    {
        var db = TestDbFactory.Create(_fixture);

        var (_, manager) = await _httpClient.PostManager<ManagerDto>();

        _httpClient.DefaultRequestHeaders.Authorization = null;

        var (createTeamResponse, team) = await _httpClient.Post<TeamDto>("/api/teams",
            new CreateTeamRequest { Name = new TeamName("Lag"), ManagerId = manager!.Id });

        Assert.Equal(HttpStatusCode.Unauthorized, createTeamResponse.StatusCode);
        Assert.Null(team);
    }
}
