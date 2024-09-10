using System.Net;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ManagerGame.Test;

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
        var createManagerRequest = new CreateManagerRequest
            { Name = new ManagerName("Jakob"), Email = new Email("jakob@jakobsson.com") };
        var db = TestDbFactory.Create(_webApplicationFactory.Services);
        
        var response = await _httpClient.Post("/api/managers", createManagerRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(db.Managers);
    }
}