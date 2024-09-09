using System.Net;
using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        var createManagerResponse = await _httpClient.PostManager();
        var manager = (await createManagerResponse.Content.ReadAsStringAsync()).Deserialize<ManagerDto>();
        var createTeamRequest = new CreateTeamRequest
            { Name = new TeamName("Jakobs lag"), ManagerId = manager!.Id };

        using var scope = _webApplicationFactory.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<ApplicationDbContext>();

        var createTeamResponse = await _httpClient.Post("/api/teams", createTeamRequest);

        Assert.Equal(HttpStatusCode.OK, createTeamResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);

        var team = (await createTeamResponse.Content.ReadAsStringAsync()).Deserialize<TeamDto>()!;
        Assert.Equal(manager.Id, team.ManagerId);
        Assert.Equal("Jakobs lag", team.Name.Name);

        Assert.Single(db!.Teams);
        var createdManagerInDb = db.Managers.Include(m => m.Teams).First(x => x.Id == manager.Id);
        var createdTeamInDb = createdManagerInDb.Teams.First(x => x.Id == team.Id);
        Assert.Equal(manager.Id, createdTeamInDb.ManagerId);
        Assert.Equal("Jakobs lag", createdTeamInDb.Name.Name);
    }
}