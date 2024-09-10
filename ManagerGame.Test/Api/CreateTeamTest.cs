using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ManagerGame.Test.Api;

public class CreateTeamTest : IClassFixture<Fixture>
{
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public CreateTeamTest(Fixture fixture)
    {
        _webApplicationFactory = fixture;
        _httpClient = _webApplicationFactory.CreateDefaultClient();
    }
    
    public string GenerateToken()
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = "492394e96d224a9a658a882ffcea948e1d213f0c08c746181f14ed3a6e7743cee9e5b71e59bed2babd7fb1e2e735301bbeb63dff1bc6fc1e7f4cef199bb183b12566ef7a429b9d8968516d2ca1452ce6e3f8478e9980db37dffc0a2d784fd461d7589d2c33fded0992df093243eeace0c0088094378a6d9161f9e432fab3660a7c8955b9ea43a1cef0c409741644567a1b515cab8f3372bb3617455d726d5cc8e9b9b35e99eca3e483be99768a07c88111b108b574330a2798c03930c0166b18c751f7b4d6973bc1599a98a08770773e42183076439ed4272ea4663ce6117e65df0162f2edd77f158b4bea7c81a34b848930f3846804923abd6d0af8efdd13b0"u8.ToArray();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", "hej") }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [Fact]
    public async Task CreateTeam()
    {
        var (createManagerResponse, manager) = await _httpClient.PostManager<ManagerDto>();
        var createTeamRequest = new CreateTeamRequest
            { Name = new TeamName("Lag"), ManagerId = manager!.Id };
        var db = TestDbFactory.Create(_webApplicationFactory.Services);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateToken());

        var (createTeamResponse, team) = await _httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        Assert.Equal(HttpStatusCode.OK, createTeamResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);

        Assert.Equal(manager.Id, team!.ManagerId);
        Assert.Equal("Lag", team.Name.Name);

        Assert.Single(db.Teams);
        var createdManagerInDb = db.Managers.Include(m => m.Teams).First(x => x.Id == manager.Id);
        var createdTeamInDb = createdManagerInDb.Teams.First(x => x.Id == team.Id);
        Assert.Equal(manager.Id, createdTeamInDb.ManagerId);
        Assert.Equal("Lag", createdTeamInDb.Name.Name);
    }
    
    [Fact]
    public async Task UnauthorizedIfNotLoggedIn()
    {
        var (_, manager) = await _httpClient.PostManager<ManagerDto>();
        
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        var (createTeamResponse, team) = await _httpClient.Post<TeamDto>("/api/teams",
            new CreateTeamRequest { Name = new TeamName("Lag"), ManagerId = manager!.Id });
        
        Assert.Equal(HttpStatusCode.Unauthorized, createTeamResponse.StatusCode);
        Assert.Null(team);
    }
}