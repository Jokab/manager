using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Domain;

public class TeamTest
{
	[Fact]
	public void CanSignNewPlayer()
	{
		var team = Team.Create(new TeamName("Lag"), Guid.NewGuid(), []);
		var player = new Player(Guid.NewGuid(), new PlayerName("Jakob"), Position.Defender);
		Assert.Empty(team.Players);

		team.SignPlayer(player);

		Assert.Contains(player, team.Players);
	}

	[Fact]
	public void CannotSignDuplicatePlayer()
	{
		var team = Team.Create(new TeamName("Lag"), Guid.NewGuid(), []);
		var player = new Player(Guid.NewGuid(), new PlayerName("Jakob"), Position.Defender);
		Assert.Empty(team.Players);

		team.SignPlayer(player);

		Assert.Throws<ArgumentException>(() => team.SignPlayer(player));
	}
}
