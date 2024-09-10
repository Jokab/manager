using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ManagerGame.Test.Api;

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
        var (createManagerResponse, manager) = await _httpClient.PostManager<ManagerDto>();
        var request = new LoginRequest { ManagerId = manager.Id };

        var (loginResponse, login) = await _httpClient.Post<LoginResponseDto>("/api/login", request);

        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        Assert.Equal(manager.Id, login.Manager.Id);
        Assert.Equal("token", login.Token);
    }
}