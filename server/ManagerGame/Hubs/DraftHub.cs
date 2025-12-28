using Microsoft.AspNetCore.SignalR;
using ManagerGame.Domain;
using ManagerGame.Core;
using ManagerGame.Core.Drafting;

namespace ManagerGame.Hubs;

public class DraftHub : Hub
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICommandHandler<PickDraftPlayerRequest, DraftPickOutcome> _pickHandler;
    private readonly ICommandHandler<StartDraftRequest, Draft> _startDraftHandler;
    private readonly ILogger<DraftHub> _logger;

    public DraftHub(
        ApplicationDbContext dbContext,
        ICommandHandler<PickDraftPlayerRequest, DraftPickOutcome> pickHandler,
        ICommandHandler<StartDraftRequest, Draft> startDraftHandler,
        ILogger<DraftHub> logger)
    {
        _dbContext = dbContext;
        _pickHandler = pickHandler;
        _startDraftHandler = startDraftHandler;
        _logger = logger;
    }

    public async Task JoinDraft(Guid draftId)
    {
        _logger.LogInformation("Client {ConnectionId} joining draft {DraftId}", Context.ConnectionId, draftId);

        var draft = await _dbContext.DraftsRepo.Find(draftId);
        if (draft is null)
        {
            _logger.LogWarning("Draft {DraftId} not found", draftId);
            await Clients.Caller.SendAsync("DraftError", "Draft not found");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, draftId.ToString());
        _logger.LogInformation("Client {ConnectionId} joined draft group {DraftId}", Context.ConnectionId, draftId);

        await Clients.Caller.SendAsync("JoinedDraft", new
        {
            DraftId = draftId,
            LeagueId = draft.LeagueId,
            State = draft.State.ToString(),
            NextTeamId = draft.PeekNextTeamId(),
            PicksPerTeam = draft.PicksPerTeam,
            TotalPicks = draft.Picks.Count
        });
    }

    public async Task StartDraft(Guid draftId, int picksPerTeam)
    {
        _logger.LogInformation("Starting draft {DraftId} with {PicksPerTeam} picks per team", draftId, picksPerTeam);

        var result = await _startDraftHandler.Handle(new StartDraftRequest
        {
            DraftId = draftId,
            PicksPerTeam = picksPerTeam
        });

        if (result.IsFailure)
        {
            _logger.LogError("Failed to start draft {DraftId}: {Error}", draftId, result.Error.Code);
            await Clients.Caller.SendAsync("DraftError", result.Error.Code);
            return;
        }

        var draft = result.Value!;
        _logger.LogInformation("Draft {DraftId} started successfully. Broadcasting DraftStarted to group.", draftId);

        // Notify all clients in this draft group that the draft has started
        await Clients.Group(draftId.ToString()).SendAsync("DraftStarted", new
        {
            DraftId = draftId,
            State = draft.State.ToString(),
            NextTeamId = draft.PeekNextTeamId(),
            PicksPerTeam = draft.PicksPerTeam,
            TotalPicksTarget = draft.TotalPicksTarget
        });

        _logger.LogInformation("DraftStarted event sent to group {DraftId}", draftId);
    }

    public async Task PickPlayer(Guid playerId, Guid draftId, Guid teamId)
    {
        var draft = await _dbContext.DraftsRepo.Find(draftId);
        if (draft is null)
        {
            await Clients.Caller.SendAsync("PickError", "Draft not found");
            return;
        }

        var player = await _dbContext.Players.FindAsync(playerId);
        if (player is null)
        {
            await Clients.Caller.SendAsync("PickError", "Player not found");
            return;
        }

        var result = await _pickHandler.Handle(new PickDraftPlayerRequest
        {
            DraftId = draftId,
            TeamId = teamId,
            PlayerId = playerId
        });

        if (result.IsFailure)
        {
            await Clients.Caller.SendAsync("PickError", result.Error.Code);
            return;
        }

        var outcome = result.Value!;

        // Notify all clients in this draft group about the pick
        await Clients.Group(draftId.ToString()).SendAsync("PlayerPicked", new
        {
            PlayerId = playerId,
            TeamId = teamId,
            TeamName = outcome.Team.Name.Name,
            NextTeamId = outcome.NextTeamId,
            PlayerName = player.Name.Name,
            PlayerPosition = player.Position.ToString(),
            PlayerCountry = player.Country.Country.ToString(),
            PickNumber = outcome.Pick.PickNumber,
            DraftState = outcome.Draft.State.ToString(),
            TotalPicks = outcome.Draft.Picks.Count
        });
    }
}
