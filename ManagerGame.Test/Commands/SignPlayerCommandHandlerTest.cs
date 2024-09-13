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
		var sut = new SignPlayerCommandHandler(new FakeRepo<Player>(), teamRepo);

		var res = await sut.Handle(new SignPlayerRequest(team.Id));

		Assert.Single(res.Value!.Players);
		Assert.Single((await teamRepo.Find(team.Id))!.Players);
	}
}
