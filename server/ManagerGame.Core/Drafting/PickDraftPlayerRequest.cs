namespace ManagerGame.Core.Drafting;

public class PickDraftPlayerRequest
{
    public Guid DraftId { get; init; }
    public Guid TeamId { get; init; }
    public Guid PlayerId { get; init; }
}


