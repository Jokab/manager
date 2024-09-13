using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class SignPlayerCommandHandler(IRepository<Player> playerRepo, IRepository<Team> teamRepo) : ICommandHandler<SignPlayerRequest, Team>
{
	public async Task<Result<Team>> Handle(SignPlayerRequest command, CancellationToken cancellationToken = default)
	{
		var team = await teamRepo.Find(command.TeamId, cancellationToken);
		if (team is null) return Result<Team>.Failure(Error.NotFound);

		var player = new Player(Guid.NewGuid(), new PlayerName("Jakob"), Position.Defender);
		team.SignPlayer(player);

		await playerRepo.Add(player, cancellationToken);

		return Result<Team>.Success(team);
	}
}
