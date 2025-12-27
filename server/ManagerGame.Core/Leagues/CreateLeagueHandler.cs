namespace ManagerGame.Core.Leagues;

public class CreateLeagueHandler(ApplicationDbContext dbContext) : ICommandHandler<CreateLeagueRequest, League>
{
    public async Task<Result<League>> Handle(CreateLeagueRequest command,
        CancellationToken cancellationToken = default)
    {
        var league = League.Empty();
        dbContext.Leagues2.Add(league);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<League>.Success(league);
    }
}
