using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ManagerGame.Test.Api;

public class LoginTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly IConfiguration _configuration;

    public LoginTest(Fixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
        this._configuration = _webApplicationFactory.Services.GetService<IConfiguration>()!;
    }

    [Fact]
    public async Task GeneratesJwtTokenForManager()
    {
        var (createManagerResponse, manager) = await _httpClient.PostManager<ManagerDto>();
        var request = new LoginRequest { ManagerId = manager!.Id };

        var (loginResponse, login) = await _httpClient.Post<LoginResponseDto>("/api/login", request);

        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Equal(manager.Id, login!.Manager.Id);
        Assert.NotNull(ValidateToken(login.Token));
    }

    private SecurityToken ValidateToken(string authToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!))
        };

        tokenHandler.ValidateToken(authToken, validationParameters, out var validatedToken);

        return validatedToken;
    }
}
