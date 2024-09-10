using System.IdentityModel.Tokens.Jwt;
using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;

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

    private static SecurityToken ValidateToken(string authToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(
                "492394e96d224a9a658a882ffcea948e1d213f0c08c746181f14ed3a6e7743cee9e5b71e59bed2babd7fb1e2e735301bbeb63dff1bc6fc1e7f4cef199bb183b12566ef7a429b9d8968516d2ca1452ce6e3f8478e9980db37dffc0a2d784fd461d7589d2c33fded0992df093243eeace0c0088094378a6d9161f9e432fab3660a7c8955b9ea43a1cef0c409741644567a1b515cab8f3372bb3617455d726d5cc8e9b9b35e99eca3e483be99768a07c88111b108b574330a2798c03930c0166b18c751f7b4d6973bc1599a98a08770773e42183076439ed4272ea4663ce6117e65df0162f2edd77f158b4bea7c81a34b848930f3846804923abd6d0af8efdd13b0"u8
                    .ToArray()) // The same key as the one that generate the token
        };

        tokenHandler.ValidateToken(authToken, validationParameters, out var validatedToken);

        return validatedToken;
    }
}