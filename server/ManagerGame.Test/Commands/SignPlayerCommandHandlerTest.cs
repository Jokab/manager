using ManagerGame.Core;
using ManagerGame.Core.Teams;
using NSubstitute;

namespace ManagerGame.Test.Commands;

public class SignPlayerCommandHandlerTest
{
    [Fact]
    public async Task PersistsPlayerInTeam()
    {
        var teamRepo = Substitute.For<IRepository<Team>>();
        Team team = TestData.TeamEmpty("Laget");
        teamRepo.Find(team.Id).Returns(team);

        var playerRepo = Substitute.For<IRepository<Player>>();
        Player player = TestData.Player();
        playerRepo.Find(player.Id).Returns(player);

        Assert.Empty(team.Players);
        Assert.Null(player.TeamId);
        var sut = new SignPlayerCommandHandler(playerRepo, teamRepo);

        await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

        await teamRepo.Received().Update(Arg.Is<Team>(t =>
            t.Players.First().Player.Id == player.Id
            && t.Players.First().TeamId == team.Id
            && t.Players.First().Player.Name.Name == "Jakob"
            && t.Players.First().Player.Position == Position.Defender
            && t.Players.First().Player.Country.Country == Country.Se));
    }

    [Fact]
    public async Task CannotSignSignedPlayer()
    {
        var teamRepo = Substitute.For<IRepository<Team>>();
        Team team = TestData.TeamEmpty("Laget");
        Team team2 = TestData.TeamEmpty("Laget2");
        teamRepo.Find(team.Id).Returns(team);
        teamRepo.Find(team2.Id).Returns(team2);

        var playerRepo = Substitute.For<IRepository<Player>>();
        Player player = TestData.Player();
        playerRepo.Find(player.Id).Returns(player);

        Assert.Empty(team.Players);
        Assert.Empty(team2.Players);
        var sut = new SignPlayerCommandHandler(playerRepo, teamRepo);
        await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

        Assert.True(player.IsSigned);

        Result<Team> signResult = await sut.Handle(new SignPlayerRequest(team2.Id, player.Id));

        Assert.True(signResult.IsFailure);
        Assert.NotEmpty(signResult.Error.Code);
    }
}
