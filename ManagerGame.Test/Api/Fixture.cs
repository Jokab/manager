using ManagerGame.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
	        var config  = new ConfigurationBuilder()
		        .AddJsonFile("appsettings.json", true, true)
		        .AddEnvironmentVariables()
		        .Build();
	        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
		        .UseNpgsql(config.GetConnectionString("Db"))
		        .Options;
            var dbContext = new ApplicationDbContext(options);
            services.AddSingleton(dbContext);
            dbContext.Database.Migrate();
        });

        return base.CreateHost(builder);
    }
}
