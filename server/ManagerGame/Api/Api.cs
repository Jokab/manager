using ManagerGame.Api.Drafting;
using ManagerGame.Api.Dtos;
using ManagerGame.Api.Leagues;
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

        api.MapPost("drafts", CreateDraft).RequireAuthorization("user");
        api.MapPost("drafts/start", StartDraft).RequireAuthorization("user");

        api.MapPost("leagues", CreateLeague).RequireAuthorization("user");
        api.MapPost("leagues/admitTeam", AdmitTeam).RequireAuthorization("user");
    }

    private static async Task<Ok<CreateDraftDto>> CreateDraft(
        CreateDraftRequest request,
        CreateDraftHandler handler)
    {
        var draft = await handler.Handle(request);

        return TypedResults.Ok(new CreateDraftDto(draft.Value!));
    }


    private static async Task<Ok<StartDraftDto>> StartDraft(
        StartDraftRequest request,
        StartDraftHandler handler)
    {
        var draft = await handler.Handle(request);

        return TypedResults.Ok(new StartDraftDto { Id = draft.Value!.Id, State = draft.Value!.State });
    }

    private static async Task<Ok<CreateLeagueDto>> CreateLeague(
        CreateLeagueRequest request,
        CreateLeagueHandler handler)
    {
        var result = await handler.Handle(request);

        return TypedResults.Ok(new CreateLeagueDto(result.Value!));
    }

    private static async Task<Ok<AdmitTeamDto>> AdmitTeam(
        AdmitTeamRequest request,
        AdmitTeamHandler handler)
    {
        var result = await handler.Handle(request);

        return TypedResults.Ok(new AdmitTeamDto(result.Value!));
    }

    private static async Task<Ok<SignPlayerDto>> SignPlayer(
        SignPlayerRequest request,
        SignPlayerCommandHandler handler)
    {
        await handler.Handle(request);

        return TypedResults.Ok(new SignPlayerDto());
    }



    private static async Task<Results<Ok<LoginResponseDto>, ProblemHttpResult>> Login(
        LoginRequest request,
        LoginCommandHandler commandHandler,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.ManagerEmail)) return TypedResults.Problem("Empty email");

        var result = await commandHandler.Handle(new LoginCommand {ManagerEmail = new Email(request.ManagerEmail)}, cancellationToken);
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
        var result = await handler.Handle(request.ToCommand(), cancellationToken);
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

    private static async Task<Ok<TeamDto>> GetTeam(Guid id,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var team = await dbContext.Teams.FindAsync([id, cancellationToken], cancellationToken);
        return TypedResults.Ok(new TeamDto(team!));
    }
}
