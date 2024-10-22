using Microsoft.AspNetCore.SignalR;

namespace ManagerGame;

public class TestHub : Hub
{
    public async Task SignedPlayer(Guid teamId,
        Guid playerId)
    {
        await Clients.All.SendAsync("signedPlayer", teamId, playerId);
    }
}
