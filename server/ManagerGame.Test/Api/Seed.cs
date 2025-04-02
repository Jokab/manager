using System.Net;
using System.Net.Http.Headers;
using ManagerGame.Api;
using ManagerGame.Api.Dtos;
using ManagerGame.Api.Requests;

namespace ManagerGame.Test.Api;

public static class Seed
{
    public static async Task<(ManagerDto manager, string token)> SeedManagerAndLogin(HttpClient httpClient)
    {
        (_, ManagerDto? manager) = await httpClient.PostManager<ManagerDto>();
        (_, LoginResponseDto? login) = await httpClient.Post<LoginResponseDto>("/api/login",
            new LoginRequest { ManagerEmail = manager!.Email.EmailAddress });

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        return (manager, login.Token);
    }

    public static async Task<(ManagerDto manager, TeamDto team)> SeedAndLogin(HttpClient httpClient)
    {
        (_, ManagerDto? manager) = await httpClient.PostManager<ManagerDto>();
        CreateTeamRequest createTeamRequest = new()
            { Name = "Lag", ManagerId = manager!.Id };
        (_, LoginResponseDto? login) =
            await httpClient.Post<LoginResponseDto>("/api/login",
                new LoginRequest { ManagerEmail = manager.Email.EmailAddress });

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        (HttpResponseMessage? createTeamResponseMessage, TeamDto? team) = await httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        Assert.Equal(HttpStatusCode.OK, createTeamResponseMessage.StatusCode);

        return (manager, team!);
    }
}
