namespace ManagerGame.Core.Leagues;

public class CreateLeagueHandler(IRepository<League> repo)
{
    public async Task<Result<League>> Handle(CreateLeagueRequest command,
        CancellationToken cancellationToken = default)
    {
        var league = League.Empty();
        await repo.Add(league, cancellationToken);

        return Result<League>.Success(league);
    }
}
