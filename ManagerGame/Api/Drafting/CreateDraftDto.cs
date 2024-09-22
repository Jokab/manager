using System.Text.Json.Serialization;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Drafting;

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
    public State State { get; set; }
}
