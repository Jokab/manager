using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ManagerGame.Services;

public class CurrentManagerService(ProtectedLocalStorage storage)
{
    private const string ManagerIdKey = "current-manager-id";

    public Guid? ManagerId { get; private set; }

    public async Task InitializeAsync()
    {
        if (ManagerId is not null)
            return;

        try
        {
            var result = await storage.GetAsync<Guid>(ManagerIdKey);
            if (result.Success)
                ManagerId = result.Value;
        }
        catch (InvalidOperationException)
        {
            // During prerender (ServerPrerendered), JS interop isn't available yet.
            // Call InitializeAsync from OnAfterRenderAsync(firstRender: true) to load from storage.
        }
    }

    public async Task SetManagerAsync(Guid managerId)
    {
        ManagerId = managerId;
        await storage.SetAsync(ManagerIdKey, managerId);
    }

    public async Task ClearAsync()
    {
        ManagerId = null;
        await storage.DeleteAsync(ManagerIdKey);
    }
}

