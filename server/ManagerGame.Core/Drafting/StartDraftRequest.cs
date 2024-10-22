namespace ManagerGame.Core.Drafting;

public class StartDraftRequest : ICommand<Draft>
{
    public Guid DraftId { get; set; }
}
