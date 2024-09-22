using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Drafting;

internal class CreateDraftHandler(IRepository<Draft> repo) : ICommandHandler<CreateDraftRequest, Draft>
{
    public async Task<Result<Draft>> Handle(CreateDraftRequest command,
        CancellationToken cancellationToken = default)
    {
        var draft = Draft.DoubledPeakTraversalDraft(new League(Guid.NewGuid()));

        var newDraft = await repo.Add(draft, cancellationToken);
        
        return Result<Draft>.Success(newDraft);
    }
}