using System.Net;
using System.Net.Http.Headers;
using ManagerGame.Api;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Api;

public static class Seed
{
    public static async Task<(ManagerDto manager, string token)> SeedManagerAndLogin(HttpClient httpClient)
    {
        var (_, manager) = await httpClient.PostManager<ManagerDto>();
        var (_, login) = await httpClient.Post<LoginResponseDto>("/api/login", new LoginRequest { ManagerEmail = manager!.Email.EmailAddress });

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        return (manager, login.Token);
    }

    public static async Task<(ManagerDto manager, TeamDto team)> SeedAndLogin(HttpClient httpClient)
    {
        var (_, manager) = await httpClient.PostManager<ManagerDto>();
        var createTeamRequest = new CreateTeamRequest
            { Name = "Lag", ManagerId = manager!.Id };
        var (_, login) =
            await httpClient.Post<LoginResponseDto>("/api/login", new LoginRequest { ManagerEmail = manager.Email.EmailAddress });

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        var (createTeamResponseMessage, team) = await httpClient.Post<TeamDto>("/api/teams", createTeamRequest);

        Assert.Equal(HttpStatusCode.OK, createTeamResponseMessage.StatusCode);

        return (manager, team!);
    }
}
