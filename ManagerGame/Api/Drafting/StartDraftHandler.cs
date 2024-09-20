using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Drafting;

internal class StartDraftHandler : ICommandHandler<StartDraftRequest, Draft>
{
    public Task<Result<Draft>> Handle(StartDraftRequest command,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}
