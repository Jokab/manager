using Microsoft.AspNetCore.SignalR;
using ManagerGame.Domain;
using ManagerGame.Core;
using ManagerGame;

namespace ManagerGame.Blazor.Hubs;

public class DraftHub : Hub
{
    private readonly IRepository<Draft> _draftRepository;
    private readonly IRepository<Player> _playerRepository;
    private readonly IRepository<Team> _teamRepository;
    private readonly ApplicationDbContext _dbContext;

    public DraftHub(
        IRepository<Draft> draftRepository,
        IRepository<Player> playerRepository,
        IRepository<Team> teamRepository,
        ApplicationDbContext dbContext)
    {
        _draftRepository = draftRepository;
        _playerRepository = playerRepository;
        _teamRepository = teamRepository;
        _dbContext = dbContext;
    }

    public async Task JoinDraft(Guid leagueId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, leagueId.ToString());
        await Clients.Group(leagueId.ToString()).SendAsync("UserJoinedDraft", Context.ConnectionId);
    }

    public async Task PickPlayer(Guid playerId, Guid draftId, Guid teamId)
    {
        var draft = await _draftRepository.Find(draftId);
        var player = await _playerRepository.Find(playerId);
        var team = await _teamRepository.Find(teamId);

        if (draft == null || player == null || team == null)
            throw new InvalidOperationException("Invalid draft, player or team ID");

        if (draft.State != DraftState.Started)
            throw new InvalidOperationException("Draft is not in started state");

        var nextTeam = draft.GetNext();
        if (!nextTeam.IsT0)
            throw new InvalidOperationException("No next team available in draft");

        if (nextTeam.AsT0.Id != teamId)
            throw new InvalidOperationException("It's not your turn to draft");

        try
        {
            team.SignPlayer(player);
            await _dbContext.SaveChangesAsync();

            // Get next team after selection
            var newNextTeam = draft.GetNext();
            Guid? nextTeamId = newNextTeam.IsT0 ? newNextTeam.AsT0.Id : null;

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
