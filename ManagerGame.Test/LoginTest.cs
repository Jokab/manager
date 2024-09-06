using System.Net;
using System.Text;
using System.Text.Json;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ManagerGame.Test;

public class LoginTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public LoginTest(Fixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
    }

    [Fact]
    public async Task Test()
    {
        var createManagerRequest = new StringContent(
            JsonSerializer.Serialize(new CreateManagerRequest
                { Name = new ManagerName("Jakob"), Email = new Email("jakob@jakobsson.com") }),
            Encoding.UTF8,
            "application/json");

        var createManagerResponse = await _httpClient.PostAsync("/api/managers", createManagerRequest);
        var manager = (await createManagerResponse.Content.ReadAsStringAsync()).Deserialize<ManagerDto>();

        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);

        var loginRequest = new StringContent(
            JsonSerializer.Serialize(new LoginRequest { ManagerId = manager!.Id }),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await _httpClient.PostAsync("/api/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var login = (await loginResponse.Content.ReadAsStringAsync()).Deserialize<LoginResponseDto>();

        Assert.Equal(manager.Id, login!.Manager.Id);
        Assert.Equal("token", login.Token);
    }
}