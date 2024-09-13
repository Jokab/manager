using ManagerGame.Api.Dtos;
using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ManagerGame.Api;

internal static class Api
{
    public static void MapApi(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("api");

        api.MapPost("login", Login);

        api.MapPost("managers", CreateManager);
        api.MapGet("managers/{id:guid}", GetManager).RequireAuthorization("user");

        api.MapPost("teams", CreateTeam).RequireAuthorization("user");
        api.MapGet("teams/{id:guid}", GetTeam).RequireAuthorization("user");
        api.MapPost("teams/sign", SignPlayer).RequireAuthorization("user");
    }

    private static async Task<Ok<SignPlayerDto>> SignPlayer(
	    SignPlayerRequest request,
	    SignPlayerCommandHandler handler)
    {
        await handler.Handle(request);

        return TypedResults.Ok<SignPlayerDto>(new SignPlayerDto());
    }

    private static async Task<Results<Ok<LoginResponseDto>, ProblemHttpResult>> Login(
        LoginRequest request,
        LoginCommandHandler commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.Handle(request, cancellationToken);
        if (result.IsSuccess)
            return TypedResults.Ok(new LoginResponseDto
                { Manager = new ManagerDto(result.Value!.Manager), Token = result.Value.Token });
        return TypedResults.Problem(result.Error.Code);
    }

    private static async Task<Results<Ok<TeamDto>, ProblemHttpResult, UnauthorizedHttpResult>> CreateTeam(
        CreateTeamRequest request,
        CreateTeamCommandHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(request, cancellationToken);

        if (result.IsSuccess) return TypedResults.Ok(new TeamDto(result.Value!));
        return TypedResults.Problem(result.Error.Code);
    }

    private static async Task<Results<Ok<ManagerDto>, ProblemHttpResult>> CreateManager(
        CreateManagerRequest request,
        CreateManagerCommandHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(request, cancellationToken);
        if (result.IsSuccess) return TypedResults.Ok(new ManagerDto(result.Value!));
        return TypedResults.Problem(result.Error.Code);
    }

    private static async Task<Ok<ManagerDto>> GetManager(Guid id,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var manager = await dbContext.Managers.FindAsync([id, cancellationToken], cancellationToken);
        return TypedResults.Ok(new ManagerDto(manager!));
    }

    private static async Task<Ok<Team>> GetTeam(Guid id,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var team = await dbContext.Teams.FindAsync([id, cancellationToken], cancellationToken);
        return TypedResults.Ok(team);
    }
}

public class SignPlayerDto
{
}
