using ManagerGame.Core;
using ManagerGame.Core.Leagues;
using ManagerGame.Core.Managers;
using ManagerGame.Core.Teams;
using ManagerGame.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test.Api;

public static class Seed
{
    public static async Task<(Manager manager, LoginResponse loginResponse)> SeedManagerAndLogin(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var registerHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();
        var loginHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginCommand, LoginResponse>>();

        var email = $"jakob{Guid.NewGuid()}@jakobsson.com";
        var managerResult = await registerHandler.Handle(new RegisterManagerCommand
        {
            Name = new ManagerName("Jakob"),
            Email = new Email(email)
        });
        Assert.True(managerResult.IsSuccess);
        var manager = managerResult.Value!;

        var loginResult = await loginHandler.Handle(new LoginCommand
        {
            ManagerEmail = new Email(email)
        });
        Assert.True(loginResult.IsSuccess);

        return (manager, loginResult.Value!);
    }

    public static async Task<(Manager manager, Guid leagueId, Team team)> SeedAndLogin(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var registerHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();
        var leagueHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateLeagueRequest, League>>();
        var teamHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTeamCommand, Team>>();

        var email = $"jakob{Guid.NewGuid()}@jakobsson.com";
        var managerResult = await registerHandler.Handle(new RegisterManagerCommand
        {
            Name = new ManagerName("Jakob"),
            Email = new Email(email)
        });
        Assert.True(managerResult.IsSuccess);
        var manager = managerResult.Value!;

        var leagueResult = await leagueHandler.Handle(new CreateLeagueRequest { Name = "Test League" });
        Assert.True(leagueResult.IsSuccess);
        var league = leagueResult.Value!;

        var teamResult = await teamHandler.Handle(new CreateTeamCommand
        {
            Name = new TeamName($"Lag-{Guid.NewGuid()}"),
            ManagerId = manager.Id,
            LeagueId = league.Id
        });
        Assert.True(teamResult.IsSuccess);

        return (manager, league.Id, teamResult.Value!);
    }
}
