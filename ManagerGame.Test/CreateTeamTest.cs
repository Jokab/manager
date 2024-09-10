using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Test;

public class CreateTeamTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public CreateTeamTest(Fixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
    }

    [Fact]
    public async Task Test()
    {
        var (createManagerResponse, manager) = await _httpClient.PostManager<ManagerDto>();
        var createTeamRequest = new CreateTeamRequest
            { Name = new TeamName("Jakobs lag"), ManagerId = manager.Id };
        var db = TestDbFactory.Create(_webApplicationFactory.Services);

        var (createTeamResponse, team) = await _httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        Assert.Equal(HttpStatusCode.OK, createTeamResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);

        Assert.Equal(manager.Id, team.ManagerId);
        Assert.Equal("Jakobs lag", team.Name.Name);

        Assert.Single(db.Teams);
        var createdManagerInDb = db.Managers.Include(m => m.Teams).First(x => x.Id == manager.Id);
        var createdTeamInDb = createdManagerInDb.Teams.First(x => x.Id == team.Id);
        Assert.Equal(manager.Id, createdTeamInDb.ManagerId);
        Assert.Equal("Jakobs lag", createdTeamInDb.Name.Name);
    }
}