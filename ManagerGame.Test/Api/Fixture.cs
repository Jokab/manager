using System.Runtime.InteropServices;
using ManagerGame.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ManagerGame.Test.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class Fixture : WebApplicationFactory<Program>
{
    private string _connStr = "";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                _connStr = $"Host=localhost;Database={Guid.NewGuid()}";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _connStr = $"Host=localhost;Database={Guid.NewGuid()};User Id=postgres;Password=1234";

            var context = CreateContext();
            context.Database.EnsureCreated();

            services.AddSingleton(context);
        });

        return base.CreateHost(builder);
    }

    public ApplicationDbContext CreateContext() =>
        new(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_connStr)
                .Options);
}
