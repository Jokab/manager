using System.Net;
using ManagerGame.Api.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ManagerGame.Test.Api;

public class CreateManagerTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public CreateManagerTest(Fixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
    }

    [Fact]
    public async Task Test()
    {
        var db = TestDbFactory.Create(_webApplicationFactory.Services);
        
        var (createManagerResponse, manager) = await _httpClient.PostManager<ManagerDto>();

        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);
        Assert.NotNull(manager);
        Assert.NotEqual(Guid.Empty, manager.Id);
        Assert.Single(db.Managers);
        Assert.NotEqual(Guid.Empty, db.Managers.First().Id);
    }
}