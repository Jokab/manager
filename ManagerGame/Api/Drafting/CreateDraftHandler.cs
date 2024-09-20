using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Drafting;

internal class CreateDraftHandler : ICommandHandler<CreateDraftRequest, Draft>
{
    public async Task<Result<Draft>> Handle(CreateDraftRequest command,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Result<Draft>.Success(Draft.DoublePeakTraversalDraft([
            Team.Create(new TeamName("apa"), Guid.NewGuid(), []), Team.Create(new TeamName("apa"), Guid.NewGuid(), [])
        ])));
    }

}