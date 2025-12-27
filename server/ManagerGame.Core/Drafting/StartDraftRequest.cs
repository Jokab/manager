namespace ManagerGame.Core.Drafting;

public class StartDraftRequest
{
    public Guid DraftId { get; init; }
    public int? PicksPerTeam { get; init; }
}
