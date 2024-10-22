using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Leagues;

internal class CreateLeagueHandler(IRepository<League> repo) : ICommandHandler<CreateLeagueRequest, League>
{
    public async Task<Result<League>> Handle(CreateLeagueRequest command,
        CancellationToken cancellationToken = default)
    {
        var league = League.Empty();
        await repo.Add(league, cancellationToken);

        return Result<League>.Success(league);
    }
}
