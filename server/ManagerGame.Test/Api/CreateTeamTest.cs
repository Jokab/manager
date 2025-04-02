using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Api.Requests;
using ManagerGame.Core;
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
        ApplicationDbContext db = TestDbFactory.Create(_fixture);
        (ManagerDto? manager, _) = await Seed.SeedManagerAndLogin(_httpClient);
        var createTeamRequest = new CreateTeamRequest { Name = "Lag2", ManagerId = manager.Id };

        (HttpResponseMessage? createTeamResponse, TeamDto? team) = await _httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        Assert.Equal(HttpStatusCode.OK, createTeamResponse.StatusCode);

        Assert.Equal(manager.Id, team!.ManagerId);
        Assert.Equal("Lag2", team.Name);

        Assert.Single(db.Teams);
        Manager createdManagerInDb = db.Managers.Include(m => m.Teams).First(x => x.Id == manager.Id);
        Team createdTeamInDb = createdManagerInDb.Teams.First(x => x.Id == team.Id);
        Assert.Equal(manager.Id, createdTeamInDb.ManagerId);
        Assert.Equal("Lag2", createdTeamInDb.Name.Name);
    }
}

public class UnauthorizedTeamTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;

    public UnauthorizedTeamTest(Fixture fixture)
    {
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task UnauthorizedIfNotLoggedIn()
    {
        (_, ManagerDto? manager) = await _httpClient.PostManager<ManagerDto>();

        _httpClient.DefaultRequestHeaders.Authorization = null;

        (HttpResponseMessage? createTeamResponse, TeamDto? team) = await _httpClient.Post<TeamDto>("/api/teams",
            new CreateTeamRequest { Name = "Lag", ManagerId = manager!.Id });

        Assert.Equal(HttpStatusCode.Unauthorized, createTeamResponse.StatusCode);
        Assert.Null(team);
    }
}
