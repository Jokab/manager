using ManagerGame.Api.Dtos;
using ManagerGame.Api.Requests;
using ManagerGame.Core;
using ManagerGame.Core.Drafting;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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

        api.MapGet("drafts/{id:guid}", GetDraft).RequireAuthorization("user");
        api.MapPost("drafts", CreateDraft).RequireAuthorization("user");
        api.MapPost("drafts/start", StartDraft).RequireAuthorization("user");

        api.MapGet("leagues/{id:guid}", GetLeague).RequireAuthorization("user");
        api.MapPost("leagues", CreateLeague).RequireAuthorization("user");
        api.MapPost("leagues/admitTeam", AdmitTeam).RequireAuthorization("user");

        api.MapGet("players", GetPlayers);
    }

    private static async Task<Ok<CreateDraftDto>> CreateDraft(
        CreateDraftRequest request,
        ICommandHandler<CreateDraftRequest, Draft> handler)
    {
        var draft = await handler.Handle(request);

        return TypedResults.Ok(new CreateDraftDto(draft.Value!));
    }


    private static async Task<Ok<StartDraftDto>> StartDraft(
        StartDraftRequest request,
        ICommandHandler<StartDraftRequest, Draft> handler)
    {
        var draft = await handler.Handle(request);

        return TypedResults.Ok(new StartDraftDto { Id = draft.Value!.Id, State = draft.Value!.State });
    }

    private static async Task<Ok<CreateLeagueDto>> CreateLeague(
        CreateLeagueRequest request,
        ICommandHandler<CreateLeagueRequest, League> handler)
    {
        var result = await handler.Handle(request);

        return TypedResults.Ok(new CreateLeagueDto(result.Value!));
    }

    private static async Task<Ok<AdmitTeamDto>> AdmitTeam(
        AdmitTeamRequest request,
        ICommandHandler<AdmitTeamRequest, League> handler)
    {
        var result = await handler.Handle(request);

        return TypedResults.Ok(new AdmitTeamDto(result.Value!));
    }

    private static async Task<Ok<SignPlayerDto>> SignPlayer(
        SignPlayerRequest request,
        ICommandHandler<SignPlayerRequest, Team> handler,
        IHubContext<TestHub> hubContext)
    {
        await handler.Handle(request);
        await hubContext.Clients.All.SendCoreAsync("signedPlayer", [request.TeamId, request.PlayerId]);

        return TypedResults.Ok(new SignPlayerDto());
    }

    private static async Task<Ok<List<PlayerDto>>> GetPlayers(ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var players = await dbContext.Players.ToListAsync(cancellationToken);

        return TypedResults.Ok(players.Select(x => new PlayerDto(x)).ToList());
    }

    private static async Task<Results<Ok<LoginResponseDto>, ProblemHttpResult>> Login(
        LoginRequest request,
        ICommandHandler<LoginCommand, LoginResponse> commandHandler1,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.ManagerEmail)) return TypedResults.Problem("Empty email");

        var result = await commandHandler1.Handle(new LoginCommand { ManagerEmail = new Email(request.ManagerEmail) },
            cancellationToken);
        if (result.IsSuccess)
            return TypedResults.Ok(new LoginResponseDto
                { Manager = new ManagerDto(result.Value!.Manager), Token = result.Value.Token });
        return TypedResults.Problem(result.Error.Code);
    }

    private static async Task<Results<Ok<TeamDto>, ProblemHttpResult, UnauthorizedHttpResult>> CreateTeam(
        CreateTeamRequest request,
        ICommandHandler<CreateTeamCommand, Team> handler, // gör alla andra såna här okså
        CancellationToken cancellationToken = default)
    {
        var result = await handler.Handle(new CreateTeamCommand
                { Name = new TeamName(request.Name!), ManagerId = request.ManagerId!.Value },
            cancellationToken);

        if (result.IsSuccess) return TypedResults.Ok(new TeamDto(result.Value!));
        return TypedResults.Problem(result.Error.Code);
    }

    private static async Task<Results<Ok<ManagerDto>, ProblemHttpResult>> CreateManager(
        CreateManagerRequest request,
        ICommandHandler<CreateManagerCommand, Manager> handler,
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

    private static async Task<Ok<DraftDto>> GetDraft(Guid id,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var draft = await dbContext.Drafts.FindAsync([id, cancellationToken], cancellationToken);
        return TypedResults.Ok(new DraftDto(draft!));
    }

    private static async Task<Ok<LeagueDto>> GetLeague(Guid id,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var league = await dbContext.Leagues.Include(x => x.Drafts).Include(x => x.Teams)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return TypedResults.Ok(new LeagueDto(league!));
    }
}
