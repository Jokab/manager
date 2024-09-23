using Microsoft.AspNetCore.SignalR;

namespace ManagerGame;

public class TestHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        Console.WriteLine("send");
        await Clients.All.SendAsync("messageReceived", user, message);
    }
}
