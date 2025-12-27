using System.Net;
using System.Net.Http.Headers;
using ManagerGame.Api;
using ManagerGame.Api.Dtos;
using ManagerGame.Api.Requests;
using ManagerGame.Core.Leagues;

namespace ManagerGame.Test.Api;

public static class Seed
{
    public static async Task<(ManagerDto manager, string token)> SeedManagerAndLogin(HttpClient httpClient)
    {
        var (_, manager) = await httpClient.PostManager<ManagerDto>();
        var (_, login) = await httpClient.Post<LoginResponseDto>("/api/login",
            new LoginRequest { ManagerEmail = manager!.Email.EmailAddress });

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        return (manager, login.Token);
    }

    public static async Task<(ManagerDto manager, Guid leagueId, TeamDto team)> SeedAndLogin(HttpClient httpClient)
    {
        var (_, manager) = await httpClient.PostManager<ManagerDto>();
        var (_, login) =
            await httpClient.Post<LoginResponseDto>("/api/login",
                new LoginRequest { ManagerEmail = manager!.Email.EmailAddress });

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        var (_, createLeagueDto) =
            await httpClient.Post<CreateLeagueDto>("/api/leagues", new CreateLeagueRequest { Name = "Test League" });
        Assert.NotNull(createLeagueDto);

        var leagueId = createLeagueDto.Id;
        CreateTeamRequest createTeamRequest = new()
            { Name = $"Lag-{Guid.NewGuid()}", ManagerId = manager!.Id, LeagueId = leagueId };

        var (createTeamResponseMessage, team) = await httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        Assert.Equal(HttpStatusCode.OK, createTeamResponseMessage.StatusCode);

        return (manager, leagueId, team!);
    }
}
