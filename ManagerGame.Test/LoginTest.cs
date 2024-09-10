using System.Net;
using ManagerGame.Api.Dtos;
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
        var createManagerResponse = await _httpClient.PostManager();
        var manager = (await createManagerResponse.Content.ReadAsStringAsync()).Deserialize<ManagerDto>();
        var request = new LoginRequest { ManagerId = manager!.Id };

        var loginResponse = await _httpClient.Post("/api/login", request);

        var login = (await loginResponse.Content.ReadAsStringAsync()).Deserialize<LoginResponseDto>();

        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        Assert.Equal(manager.Id, login!.Manager.Id);
        Assert.Equal("token", login.Token);
    }
}