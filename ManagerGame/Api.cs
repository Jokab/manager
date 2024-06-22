using Microsoft.AspNetCore.Http.HttpResults;

namespace ManagerGame;

internal static class Api
{
    public static RouteGroupBuilder MapApi(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("api");
        api.MapPost("teams", CreateTeam);
        api.MapPost("managers", CreateManager);
        api.MapGet("teams/{id:guid}", GetTeam);
        api.MapGet("managers/{id:guid}", GetManager);

        return api;
    }

    private static async Task<Results<Ok<Team>, NotFound<string>>> CreateTeam(CreateTeamRequest request, ApplicationDbContext dbContext)
    {
        var team = new Team(request.Name, Guid.NewGuid());

        var manager = await dbContext.Managers.FindAsync(team.Id);
        if (manager == null)
        {
            return TypedResults.NotFound("Manager not found");
        }
        manager.AddTeam(team);

        await dbContext.SaveChangesAsync();
        
        return TypedResults.Ok(team);
    }
    
    private static async Task<Ok<Manager>> CreateManager(CreateEmailRequest request, ApplicationDbContext dbContext)
    {
        var manager = new Manager(request.Name, request.Email);

        dbContext.Managers.Add(manager);
        await dbContext.SaveChangesAsync();
        
        return TypedResults.Ok(manager);
    }

    private static async Task<Ok<Manager>> GetManager(Guid id, ApplicationDbContext dbContext)
    {
        var manager = await dbContext.Managers.FindAsync(id);
        return TypedResults.Ok(manager);
    }

    private static async Task<Ok<Team>> GetTeam(Guid id, ApplicationDbContext dbContext)
    {
        var team = await dbContext.Teams.FindAsync(id);
        return TypedResults.Ok(team);
    }
}

internal class CreateEmailRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
}