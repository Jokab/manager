using System.Net;
using System.Net.Http.Headers;
using ManagerGame.Api;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Test.Api;

public class CreateTeamTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;

    public CreateTeamTest(Fixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task CreateTeam()
    {
        var db = TestDbFactory.Create(_fixture);
        var (manager, _) = await Seed.SeedManagerAndLogin(_httpClient);
        var createTeamRequest = new CreateTeamRequest { Name = new TeamName("Lag2"), ManagerId = manager.Id };

        var (createTeamResponse, team) = await _httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        Assert.Equal(HttpStatusCode.OK, createTeamResponse.StatusCode);

        Assert.Equal(manager.Id, team!.ManagerId);
        Assert.Equal("Lag2", team.Name.Name);

        Assert.Single(db.Teams);
        var createdManagerInDb = db.Managers.Include(m => m.Teams).First(x => x.Id == manager.Id);
        var createdTeamInDb = createdManagerInDb.Teams.First(x => x.Id == team.Id);
        Assert.Equal(manager.Id, createdTeamInDb.ManagerId);
        Assert.Equal("Lag2", createdTeamInDb.Name.Name);
    }
}

public class UnauthorizedTeamTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;

    public UnauthorizedTeamTest(Fixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task UnauthorizedIfNotLoggedIn()
    {
        var (_, manager) = await _httpClient.PostManager<ManagerDto>();

        _httpClient.DefaultRequestHeaders.Authorization = null;

        var (createTeamResponse, team) = await _httpClient.Post<TeamDto>("/api/teams",
            new CreateTeamRequest { Name = new TeamName("Lag"), ManagerId = manager!.Id });

        Assert.Equal(HttpStatusCode.Unauthorized, createTeamResponse.StatusCode);
        Assert.Null(team);
    }
}
