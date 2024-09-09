using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ManagerGame;

internal static class Api
{
    public static RouteGroupBuilder MapApi(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("api");

        api.MapPost("login", Login);

        api.MapPost("managers", CreateManager);
        api.MapGet("managers/{id:guid}", GetManager);

        api.MapPost("teams", CreateTeam);
        api.MapGet("teams/{id:guid}", GetTeam);

        return api;
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


    private static async Task<Results<Ok<TeamDto>, ProblemHttpResult>> CreateTeam(
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