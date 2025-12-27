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
        var team = TestData.TeamEmpty("Laget");
        teamRepo.Find(team.Id).Returns(team);

        var playerRepo = Substitute.For<IRepository<Player>>();
        var player = TestData.Player();
        playerRepo.Find(player.Id).Returns(player);

        var teamPlayerRepo = Substitute.For<IRepository<TeamPlayer>>();

        Assert.Empty(team.Players);
        Assert.Null(player.TeamId);
        var sut = new SignPlayerCommandHandler(playerRepo, teamRepo, teamPlayerRepo);

        await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

        await teamPlayerRepo.Received().Add(Arg.Is<TeamPlayer>(tp =>
            tp.Player.Id == player.Id
            && tp.TeamId == team.Id
            && tp.Player.Name.Name == "Jakob"
            && tp.Player.Position == Position.Defender
            && tp.Player.Country.Country == Country.Se));
    }

    [Fact]
    public async Task CannotSignSignedPlayer()
    {
        var teamRepo = Substitute.For<IRepository<Team>>();
        var team = TestData.TeamEmpty("Laget");
        var team2 = TestData.TeamEmpty("Laget2");
        teamRepo.Find(team.Id).Returns(team);
        teamRepo.Find(team2.Id).Returns(team2);

        var playerRepo = Substitute.For<IRepository<Player>>();
        var player = TestData.Player();
        playerRepo.Find(player.Id).Returns(player);

        var teamPlayerRepo = Substitute.For<IRepository<TeamPlayer>>();

        Assert.Empty(team.Players);
        Assert.Empty(team2.Players);
        var sut = new SignPlayerCommandHandler(playerRepo, teamRepo, teamPlayerRepo);
        await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

        Assert.True(player.IsSigned);

        var signResult = await sut.Handle(new SignPlayerRequest(team2.Id, player.Id));

        Assert.True(signResult.IsFailure);
        Assert.NotEmpty(signResult.Error.Code);
    }
}
