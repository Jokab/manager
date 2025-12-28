using ManagerGame.Core;
using ManagerGame.Core.Managers;
using ManagerGame.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerGame.Test.Api;

public class RegisterManagerTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;

    public RegisterManagerTest(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Test()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterManagerCommand, Manager>>();

        var email = $"jakob{Guid.NewGuid()}@jakobsson.com";
        var result = await handler.Handle(new RegisterManagerCommand
        {
            Name = new ManagerName("Jakob"),
            Email = new Email(email)
        });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Jakob", result.Value.Name.Name);
        Assert.Equal(email, result.Value.Email.EmailAddress);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        
        db.ChangeTracker.Clear();
        Assert.Single(db.Managers);
        Assert.NotEqual(Guid.Empty, db.Managers.First().Id);
    }
}
