using System.Text.Json.Serialization;

namespace ManagerGame.Api.Dtos;

public record DraftDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DraftDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public DraftDto(Draft draft)
    {
        Id = draft.Id;
        LeagueId = draft.LeagueId;
        State = draft.State;
        CreatedDate = draft.CreatedDate;
        UpdatedDate = draft.CreatedDate;
        DeletedDate = draft.DeletedDate;
        ParticipantTeamIds = draft.Participants
            .OrderBy(x => x.Seat)
            .Select(x => x.TeamId)
            .ToList();
        Picks = draft.Picks
            .OrderBy(x => x.PickNumber)
            .Select(x => new DraftPickDto(x))
            .ToList();
    }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public Guid LeagueId { get; set; }
    public List<Guid> ParticipantTeamIds { get; set; }
    public List<DraftPickDto> Picks { get; set; }
    public DraftState State { get; private set; }
}

public record DraftPickDto
{
    [JsonConstructor]
    public DraftPickDto()
    {
    }

    public DraftPickDto(DraftPick pick)
    {
        Id = pick.Id;
        PickNumber = pick.PickNumber;
        TeamId = pick.TeamId;
        PlayerId = pick.PlayerId;
        PickedAt = pick.PickedAt;
    }

    public Guid Id { get; set; }
    public int PickNumber { get; set; }
    public Guid TeamId { get; set; }
    public Guid PlayerId { get; set; }
    public DateTime PickedAt { get; set; }
}

public record DraftPickResultDto
{
    [JsonConstructor]
    public DraftPickResultDto()
    {
    }

    public DraftPickResultDto(Draft draft, DraftPick pick, Guid? nextTeamId)
    {
        DraftId = draft.Id;
        LeagueId = draft.LeagueId;
        Pick = new DraftPickDto(pick);
        NextTeamId = nextTeamId;
        DraftState = draft.State;
    }

    public Guid DraftId { get; set; }
    public Guid LeagueId { get; set; }
    public DraftPickDto Pick { get; set; } = null!;
    public Guid? NextTeamId { get; set; }
    public DraftState DraftState { get; set; }
}
