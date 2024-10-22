using System.Text.Json.Serialization;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Dtos;

internal record DraftDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DraftDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public DraftDto(Draft draft)
    {
        Teams = draft.Teams;
        Id = draft.Id;
        LeagueId = draft.LeagueId;
        State = draft.State;
        CreatedDate = draft.CreatedDate;
        UpdatedDate = draft.CreatedDate;
        DeletedDate = draft.DeletedDate;
    }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public Guid LeagueId { get; set; }
    public ICollection<Team> Teams { get; }
    public DraftState State { get; private set; }
}
