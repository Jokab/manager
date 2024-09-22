using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

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
        var db = TestDbFactory.Create(_fixture);

        var (manager, newTeam) = await Seed.SeedAndLogin(_httpClient);

        var player = TestData.Player();
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var (httpResponseMessage, signPlayerDto) =
            await _httpClient.Post<SignPlayerDto>("/api/teams/sign", new SignPlayerRequest(newTeam.Id, player.Id));

        var (_, team) = await _httpClient.Get<TeamDto>($"/api/teams/{newTeam.Id}");

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Single(team!.Players);

        Assert.Equal("Jakob", team.Players.First().Name.Name);
        Assert.Equal(Position.Defender, team.Players.First().Position);
        Assert.Equal(team.Id, team.Players.First().TeamId);
        Assert.Equal(Country.Se, team.Players.First().Country.Country);
    }
}
