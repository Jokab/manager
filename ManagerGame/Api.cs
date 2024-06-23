using ManagerGame.Commands;
using ManagerGame.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ManagerGame;

internal static class Api
{
    public static RouteGroupBuilder MapApi(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("api");
        
        api.MapPost("managers", CreateManager);
        api.MapGet("managers/{id:guid}", GetManager);

        api.MapPost("teams", CreateTeam);
        api.MapGet("teams/{id:guid}", GetTeam);

        return api;
    }

    private static async Task<Results<Ok, ProblemHttpResult>> CreateTeam(
        CreateTeamRequest request, 
        CreateTeamCommandHandler handler, 
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(request, cancellationToken);
        if (result.IsSuccess) return TypedResults.Ok();
        return TypedResults.Problem(result.Error.Code);
    }
    
    private static async Task<Results<Ok<Manager>, ProblemHttpResult>> CreateManager(
        CreateManagerRequest request, 
        CreateManagerCommandHandler handler, 
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(request, cancellationToken);
        if (result.IsSuccess) return TypedResults.Ok(result.Value);
        return TypedResults.Problem(result.Error.Code);
    }

    private static async Task<Ok<Manager>> GetManager(Guid id, ApplicationDbContext dbContext, 
        CancellationToken cancellationToken = default)
    {
        var manager = await dbContext.Managers.FindAsync([id, cancellationToken], cancellationToken: cancellationToken);
        return TypedResults.Ok(manager);
    }

    private static async Task<Ok<Team>> GetTeam(Guid id, ApplicationDbContext dbContext, 
        CancellationToken cancellationToken = default)
    {
        var team = await dbContext.Teams.FindAsync(id, cancellationToken);
        return TypedResults.Ok(team);
    }
}