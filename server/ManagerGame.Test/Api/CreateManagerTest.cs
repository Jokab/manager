using System.Net;
using ManagerGame.Api.Dtos;

namespace ManagerGame.Test.Api;

public class CreateManagerTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;

    public CreateManagerTest(Fixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task Test()
    {
        var db = TestDbFactory.Create(_fixture);

        var (createManagerResponse, manager) = await _httpClient.PostManager<ManagerDto>();

        Assert.Equal(HttpStatusCode.OK, createManagerResponse.StatusCode);
        Assert.NotNull(manager);
        Assert.Equal("Jakob", manager.Name.Name);
        Assert.Contains("@jakobsson.com", manager.Email.EmailAddress);
        Assert.NotEqual(Guid.Empty, manager.Id);
        Assert.Single(db.Managers);
        Assert.NotEqual(Guid.Empty, db.Managers.First().Id);
    }
}
