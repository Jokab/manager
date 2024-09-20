using System.Net;
using ManagerGame.Api.Drafting;

namespace ManagerGame.Test.Api;

public class CreateDraftTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;

    public CreateDraftTest(Fixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task Test()
    {
        var db = TestDbFactory.Create(_fixture);

        var (manager, team) = await Seed.SeedAndLogin(_httpClient);

        var (http, createDraftDto) = await _httpClient.Post<CreateDraftDto>("/api/drafts", new CreateDraftRequest());

        Assert.Equal(HttpStatusCode.OK, http.StatusCode);
        Assert.NotNull(createDraftDto);
    }
}
