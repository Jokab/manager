using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Core;
using ManagerGame.Core.Teams;

namespace ManagerGame.Test.Api;

public class SignPlayerTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;


    public SignPlayerTest(Fixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
    }

    [Fact]
    public async Task SignFirstPlayer()
    {
        ApplicationDbContext db = TestDbFactory.Create(_fixture);

        (_, TeamDto? newTeam) = await Seed.SeedAndLogin(_httpClient);

        Player player = TestData.Player();
        db.Players.Add(player);
        await db.SaveChangesAsync();

        (HttpResponseMessage? httpResponseMessage, _) =
            await _httpClient.Post<SignPlayerDto>("/api/teams/sign", new SignPlayerRequest(newTeam.Id, player.Id));

        (_, TeamDto? team) = await _httpClient.Get<TeamDto>($"/api/teams/{newTeam.Id}");

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Single(team!.Players);

        Assert.Equal("Jakob", team.Players.First().Name.Name);
        Assert.Equal(Position.Defender, team.Players.First().Position);
        Assert.Equal(team.Id, team.Players.First().TeamId);
        Assert.Equal(Country.Se, team.Players.First().Country.Country);
    }
}
