using ManagerGame.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ManagerGame.Test.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class Fixture : WebApplicationFactory<Program>
{
    private readonly string _dbFilePath = Path.Combine(Path.GetTempPath(), $"manager-test-{Guid.NewGuid()}.db");
    private string SqliteConnStr => $"Data Source={_dbFilePath}";
    private bool _dbInitialized;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Minimal-hosting WebApplicationFactory config hooks can be finicky; environment variables are reliable.
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
        Environment.SetEnvironmentVariable("ASPNETCORE_DETAILEDERRORS", "true");
        Environment.SetEnvironmentVariable("Test__Sqlite__ConnectionString", SqliteConnStr);

        builder.UseEnvironment("Test");

        var host = base.CreateHost(builder);

        if (!_dbInitialized)
        {
            _dbInitialized = true;
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        }

        return host;
    }

    public ApplicationDbContext CreateContext() =>
        new(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(SqliteConnStr)
                .Options);

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        Environment.SetEnvironmentVariable("Test__Sqlite__ConnectionString", null);
        Environment.SetEnvironmentVariable("ASPNETCORE_DETAILEDERRORS", null);
        // Leave ASPNETCORE_ENVIRONMENT/DOTNET_ENVIRONMENT alone to avoid surprises in local runs.

        try
        {
            if (File.Exists(_dbFilePath))
                File.Delete(_dbFilePath);
        }
        catch
        {
            // ignore cleanup failures
        }
    }
}
