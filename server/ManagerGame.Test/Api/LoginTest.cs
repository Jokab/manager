using ManagerGame.Core;
using ManagerGame.Core.Managers;
using ManagerGame.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test.Api;

public class LoginTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;

    public LoginTest(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CanLoginWithManagerEmail()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var registerHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();
        var loginHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginCommand, LoginResponse>>();

        // Register manager
        var email = $"jakob{Guid.NewGuid()}@jakobsson.com";
        var managerResult = await registerHandler.Handle(new RegisterManagerCommand
        {
            Name = new ManagerName("Jakob"),
            Email = new Email(email)
        });
        Assert.True(managerResult.IsSuccess);
        var manager = managerResult.Value!;

        // Login
        var loginResult = await loginHandler.Handle(new LoginCommand
        {
            ManagerEmail = new Email(email)
        });

        Assert.True(loginResult.IsSuccess);
        Assert.NotNull(loginResult.Value);
        Assert.Equal(manager.Id, loginResult.Value.Manager.Id);
        Assert.NotNull(loginResult.Value.Token);
    }
}
