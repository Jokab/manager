using System.Text.Json.Serialization;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Dtos;

internal record LeagueDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public LeagueDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public LeagueDto(League league)
    {
        Teams = league.Teams.Select(x => new TeamDto(x)).ToList();
        Drafts = league.Drafts.Select(x => new DraftDto(x)).ToList();
        Id = league.Id;
        CreatedDate = league.CreatedDate;
        UpdatedDate = league.CreatedDate;
        DeletedDate = league.DeletedDate;
    }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public List<TeamDto> Teams { get; }
    public List<DraftDto> Drafts { get; }
}
