using System.Runtime.InteropServices;
using ManagerGame.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ManagerGame.Test.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class Fixture : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
	        var connStr = "";
	        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
	        {
		        connStr = $"Host=localhost;Database={Guid.NewGuid()}";
	        }
	        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
	        {
		        connStr = $"Host=localhost;Database={Guid.NewGuid()};User Id=postgres;Password=1234";
	        }
	        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
		        .UseNpgsql(connStr)
		        .Options;
            var dbContext = new ApplicationDbContext(options);
            services.AddSingleton(dbContext);
            dbContext.Database.Migrate();
        });

        return base.CreateHost(builder);
    }
}
