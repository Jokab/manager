using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Commands;

public class SignPlayerCommandHandlerTest
{
	[Fact]
	public async Task PersistsPlayerInTeam()
	{
		var teamRepo = new FakeRepo<Team>();
		var team = Team.Create(new TeamName("Laget"), Guid.NewGuid(), []);
		await teamRepo.Add(team);
		var playerRepo = new FakeRepo<Player>();
		var player = new Player(Guid.NewGuid(), team.Id, new PlayerName("Jakob"), Position.Defender);
		await playerRepo.Add(player);
		var sut = new SignPlayerCommandHandler(playerRepo, teamRepo);

		await sut.Handle(new SignPlayerRequest(team.Id, player.Id));

		var signedPlayer = (await teamRepo.Find(team.Id))!.Players.First();
		Assert.Equal(player.Id, signedPlayer.Id);
		Assert.Equal("Jakob", signedPlayer.Name.Name);
		Assert.Equal(Position.Defender, signedPlayer.Position);
	}
}
