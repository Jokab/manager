using Microsoft.AspNetCore.SignalR;
using ManagerGame.Domain;
using ManagerGame.Core;
using ManagerGame;

namespace ManagerGame.Hubs;

public class DraftHub : Hub
{
    private readonly ApplicationDbContext _dbContext;

    public DraftHub(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task JoinDraft(Guid leagueId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, leagueId.ToString());
        await Clients.Group(leagueId.ToString()).SendAsync("UserJoinedDraft", Context.ConnectionId);
    }

    public async Task PickPlayer(Guid playerId, Guid draftId, Guid teamId)
    {
        var draft = await _dbContext.Drafts2.Find(draftId);
        var player = await _dbContext.Players.FindAsync(playerId);
        var team = await _dbContext.Teams2.Find(teamId);

        if (draft == null || player == null || team == null)
            throw new InvalidOperationException("Invalid draft, player or team ID");

        if (draft.State != DraftState.Started)
            throw new InvalidOperationException("Draft is not in started state");

        var expectedTeamId = draft.PeekNextTeamId();
        if (expectedTeamId is null)
            throw new InvalidOperationException("No next team available in draft");

        if (expectedTeamId.Value != teamId)
            throw new InvalidOperationException("It's not your turn to draft");

        try
        {
            team.SignPlayer(player);
            await _dbContext.SaveChangesAsync();

            // Advance draft turn after selection (draft aggregate is the source of truth for order)
            var nextTeamId = draft.AdvanceAndGetNextTeamId();
            await _dbContext.SaveChangesAsync();

            await Clients.Group(draft.LeagueId.ToString()).SendAsync("PlayerPicked",
                new {
                    PlayerId = playerId,
                    TeamId = teamId,
                    NextTeamId = nextTeamId,
                    PlayerName = player.Name.Name,
                    PlayerPosition = player.Position,
                    PlayerCountry = player.Country.Country
                });
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("PickError", ex.Message);
        }
    }
}
