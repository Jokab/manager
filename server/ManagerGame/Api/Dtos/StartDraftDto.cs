namespace ManagerGame.Api.Dtos;

public class StartDraftDto
{
    public Guid Id { get; set; }
    public DraftState State { get; set; }
}
