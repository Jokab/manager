using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test;

public class CreateManagerTest : IClassFixture<Fixture>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _httpClient;

    public CreateManagerTest(Fixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
    }

    [Fact]
    public async Task Test()
    {
        var content = new StringContent(
            JsonSerializer.Serialize(new CreateManagerRequest { Name = "Jakob", Email = "jakob@jakobsson.com" }),
            Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/managers", content);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        using var scope = _webApplicationFactory.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<ApplicationDbContext>();

        Assert.Single(db!.Managers);
    }
}