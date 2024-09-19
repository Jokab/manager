using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;
using NSubstitute;

namespace ManagerGame.Test.Commands;

public class SignPlayerCommandHandlerTest
{
    [Fact]
    public async Task PersistsPlayerInTeam()
    {
        var teamRepo = Substitute.For<IRepository<Team>>();
        var team = Team.Create(new TeamName("Laget"), Guid.NewGuid(), []);
        teamRepo.Find(team.Id).Returns(team);

        var playerRepo = Substitute.For<IRepository<Player>>();
        var player = TestData.Player();
        playerRepo.Find(player.Id).Returns(player);

        Assert.Empty(team.Players);
        Assert.Null(player.TeamId);
        var sut = new SignPlayerCommandHandler(playerRepo, teamRepo);

        await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

        await teamRepo.Received().Update(Arg.Is<Team>(t =>
            t.Players.First().Id == player.Id
            && t.Players.First().TeamId == team.Id
            && t.Players.First().Name.Name == "Jakob"
            && t.Players.First().Position == Position.Defender
            && t.Players.First().Country.Country == Country.Se));
    }
    
    [Fact]
    public async Task CannotSignSignedPlayer()
    {
        var teamRepo = Substitute.For<IRepository<Team>>();
        var team = Team.Create(new TeamName("Laget"), Guid.NewGuid(), []);
        var team2 = Team.Create(new TeamName("Laget2"), Guid.NewGuid(), []);
        teamRepo.Find(team.Id).Returns(team);
        teamRepo.Find(team2.Id).Returns(team2);

        var playerRepo = Substitute.For<IRepository<Player>>();
        var player = TestData.Player();
        playerRepo.Find(player.Id).Returns(player);

        Assert.Empty(team.Players);
        Assert.Empty(team2.Players);
        var sut = new SignPlayerCommandHandler(playerRepo, teamRepo);
        await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

        var signResult = await sut.Handle(new SignPlayerRequest(team2.Id, player.Id));
        
        Assert.True(signResult.IsFailure);
        Assert.True(signResult.Error == Error.PlayerAlreadySigned);
    }
}
