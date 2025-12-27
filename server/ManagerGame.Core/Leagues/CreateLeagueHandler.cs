namespace ManagerGame.Core.Leagues;

public class CreateLeagueHandler(ApplicationDbContext dbContext) : ICommandHandler<CreateLeagueRequest, League>
{
    public async Task<Result<League>> Handle(CreateLeagueRequest command,
        CancellationToken cancellationToken = default)
    {
        var settings = new LeagueSettings(
            command.MaxPlayersFromSameCountry,
            command.PointsPerGoal,
            command.PointsPerWin,
            command.PointsPerAssist,
            command.PointsPerCleanSheet);

        var league = League.Create(command.Name, settings);
        dbContext.Leagues2.Add(league);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<League>.Success(league);
    }
}
