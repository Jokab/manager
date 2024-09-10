using ManagerGame.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test;

public static class TestDbFactory
{
    public static ApplicationDbContext Create(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
        if (db is null) throw new Exception("Failed to acquire db");
        return db;
    }
}