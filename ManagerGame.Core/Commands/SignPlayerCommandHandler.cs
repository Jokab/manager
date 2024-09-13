using ManagerGame.Core.Domain;

namespace ManagerGame.Core.Commands;

public class SignPlayerCommandHandler(ApplicationDbContext dbContext) : ICommandHandler<SignPlayerRequest, Team>
{
	public async Task<Result<Team>> Handle(SignPlayerRequest command, CancellationToken cancellationToken = default)
	{
		var team = await dbContext.Teams.FindAsync([command.TeamId], cancellationToken);
		if (team is null) return Result<Team>.Failure(Error.NotFound);

		var player = new Player(Guid.NewGuid(), new PlayerName("Jakob"), Position.Defender);
		team.SignPlayer(player);

		dbContext.Add(player);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result<Team>.Success(team);
	}
}
