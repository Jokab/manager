using System.Text.Json.Serialization;
using ManagerGame.Api.Drafting;
using ManagerGame.Api.Dtos;
using ManagerGame.Api.Leagues;
using ManagerGame.Core;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
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
        SignPlayerCommandHandler handler,
        IHubContext<TestHub> hubContext)
    {
        await handler.Handle(request);
        await hubContext.Clients.All.SendCoreAsync("signedPlayer", [request.TeamId, request.PlayerId]);

        return TypedResults.Ok(new SignPlayerDto());
    }

    private static async Task<Ok<List<PlayerDto>>> GetPlayers(ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var players = await dbContext.Players.ToListAsync(cancellationToken);

        return TypedResults.Ok(players.Select(x => new PlayerDto(x)).ToList());
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
        var result = await handler.Handle(new CreateTeamCommand
                { Name = new TeamName(request.Name!), ManagerId = request.ManagerId!.Value },
            cancellationToken);

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

internal record PlayerDto
{
    public PlayerDto(Player player)
    {
        Id = player.Id;
        CreatedDate = player.CreatedDate;
        UpdatedDate = player.CreatedDate;
        DeletedDate = player.DeletedDate;
        Country = player.Country.Country.ToString();
        Name = player.Name.Name;
        TeamId = player.TeamId;
        Position = player.Position.ToString();
        IsSigned = player.IsSigned;
    }

    public DateTime? DeletedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid Id { get; set; }
    public Guid? TeamId { get; set; }
    public string Name { get; init; }
    public string Position { get; init; }
    public string Country { get; init; }
    public bool IsSigned { get; set; }

    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public PlayerDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

internal class LeagueDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public LeagueDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public LeagueDto(League league)
    {
        Teams = league.Teams.Select(x => new TeamDto(x)).ToList();
        Drafts = league.Drafts.Select(x => new DraftDto(x)).ToList();
        Id = league.Id;
        CreatedDate = league.CreatedDate;
        UpdatedDate = league.CreatedDate;
        DeletedDate = league.DeletedDate;
    }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public List<TeamDto> Teams { get; }
    public List<DraftDto> Drafts { get; }
}

internal class DraftDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DraftDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public DraftDto(Draft draft)
    {
        Teams = draft.Teams;
        Id = draft.Id;
        LeagueId = draft.LeagueId;
        State = draft.State;
        CreatedDate = draft.CreatedDate;
        UpdatedDate = draft.CreatedDate;
        DeletedDate = draft.DeletedDate;
    }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public Guid LeagueId { get; set; }
    public ICollection<Team> Teams { get; }
    public State State { get; private set; }
}
