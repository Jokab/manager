using System.Text.Json.Serialization;

namespace ManagerGame.Api.Dtos;

public class CreateDraftDto
{
    [JsonConstructor]
    public CreateDraftDto()
    {
    }

    public CreateDraftDto(Draft draft)
    {
        Id = draft.Id;
        State = draft.State;
    }

    public Guid Id { get; set; }
    public DraftState State { get; set; }
}
