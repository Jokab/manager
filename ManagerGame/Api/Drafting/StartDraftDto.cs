using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Drafting;

public class StartDraftDto
{
    public Guid Id { get; set; }
    public State State { get; set; }
}