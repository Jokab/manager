using System.Text.Json.Serialization;
using ManagerGame.Commands;
using ManagerGame.Domain;
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
                { Manager = new ManagerDto(result.Value.Manager), Token = result.Value.Token });
        return TypedResults.Problem(result.Error.Code);
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

    private static async Task<Results<Ok<ManagerDto>, ProblemHttpResult>> CreateManager(
        CreateManagerRequest request,
        CreateManagerCommandHandler handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(request, cancellationToken);
        if (result.IsSuccess) return TypedResults.Ok(new ManagerDto(result.Value));
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
        var team = await dbContext.Teams.FindAsync([id, cancellationToken], cancellationToken: cancellationToken);
        return TypedResults.Ok(team);
    }
}

public sealed record LoginResponse(Manager Manager, string Token);

public class LoginRequest : ICommand<LoginResponse>
{
    public Guid ManagerId { get; set; }
}

public record ManagerDto
{
    [JsonConstructor]
    public ManagerDto() { }

    public ManagerDto(Manager manager)
    {
        Id = manager.Id;
        CreatedDate = manager.CreatedDate;
        UpdatedDate = manager.CreatedDate;
        DeletedDate = manager.DeletedDate;
        Name = manager.Name;
        Email = manager.Email;
    }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }

    public ManagerName Name { get; set; }
    public Email Email { get; set; }
    public List<Team> Teams { get; init; } = [];
}

public class LoginResponseDto
{
    public ManagerDto Manager { get; set; }
    public string Token { get; set; }
}