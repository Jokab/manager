using System.Net;
using System.Text;
using System.Text.Json;
using ManagerGame.Commands;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test;

public class CreateTeamTest : IClassFixture<Fixture>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _httpClient;

    public CreateTeamTest(Fixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
    }

    [Fact]
    public async Task Test()
    {
	    var createManagerRequest = new StringContent(
		    JsonSerializer.Serialize(new CreateManagerRequest { Name = "Jakob", Email = "jakob@jakobsson.com" }),
		    Encoding.UTF8, "application/json");

	    var createManagerResponse = await _httpClient.PostAsync("/api/managers", createManagerRequest);
	    var manager = (await createManagerResponse.Content.ReadAsStringAsync()).Deserialize<ManagerDto>();

	    Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);

        var content = new StringContent(
            JsonSerializer.Serialize(new CreateTeamRequest { Name = "Jakobs lag", ManagerId = manager!.Id}),
            Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/teams", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _webApplicationFactory.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<ApplicationDbContext>();

        Assert.Single(db!.Teams);
    }
}
