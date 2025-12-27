using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using ManagerGame.Api;
using ManagerGame.Api.Dtos;
using ManagerGame.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ManagerGame.Test.Api;

public class LoginTest : IClassFixture<Fixture>
{
    private readonly IConfiguration _configuration;
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;

    public LoginTest(Fixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
        _configuration = fixture.Services.GetService<IConfiguration>()!;
    }

    [Fact]
    public async Task GeneratesJwtTokenForManager()
    {
        var db = TestDbFactory.Create(_fixture);

        var (createManagerResponse, manager) = await _httpClient.PostManager<ManagerDto>();
        var request = new LoginRequest { ManagerEmail = manager!.Email.EmailAddress };

        var (loginResponse, login) = await _httpClient.Post<LoginResponseDto>("/api/login", request);

        db.ChangeTracker.Clear();

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
