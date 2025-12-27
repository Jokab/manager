using System.Net;
using ManagerGame.Api.Dtos;
using ManagerGame.Core;
using ManagerGame.Core.Teams;
using Xunit;

namespace ManagerGame.Test.Api;

public class SignPlayerTest : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;


    public SignPlayerTest(Fixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateDefaultClient();
        _output = output;
    }

    [Fact]
    public async Task SignFirstPlayer()
    {
        var db = TestDbFactory.Create(_fixture);

        var (_, _, newTeam) = await Seed.SeedAndLogin(_httpClient);

        var player = TestData.Player();
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var (httpResponseMessage, _) =
            await _httpClient.Post<SignPlayerDto>("/api/teams/sign", new SignPlayerRequest(newTeam.Id, player.Id));

        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
        {
            var body = await httpResponseMessage.Content.ReadAsStringAsync();
            _output.WriteLine($"POST /api/teams/sign failed: {(int)httpResponseMessage.StatusCode} {httpResponseMessage.StatusCode}");
            _output.WriteLine(body);
        }

        var (_, team) = await _httpClient.Get<TeamDto>($"/api/teams/{newTeam.Id}");

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Single(team!.Players);

        Assert.Equal("Jakob", team.Players.First().Name);
        Assert.Equal(Position.Defender.ToString(), team.Players.First().Position);
        Assert.Equal(Country.Se.ToString(), team.Players.First().Country);
    }
}
