using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Drafting;

public class StartDraftDto
{
    public Guid Id { get; set; }
    public DraftState State { get; set; }
}
