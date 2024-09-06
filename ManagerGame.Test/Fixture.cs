using ManagerGame.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ManagerGame.Test;

public sealed class Fixture : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql($"Host=localhost;Database={Guid.NewGuid().ToString()};User Id=postgres;Password=1234")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            services.AddSingleton(dbContext);
            dbContext.Database.Migrate();
        });

        return base.CreateHost(builder);
    }
}