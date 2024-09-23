using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Drafting;

public class StartDraftRequest : ICommand<Draft>
{
    public Guid DraftId { get; set; }
}
