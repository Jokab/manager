using System.Text.Json.Serialization;

namespace ManagerGame.Api.Dtos;

public record AdmitTeamDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AdmitTeamDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public AdmitTeamDto(League league)
    {
        LeagueId = league.Id;
        Teams = league.Teams.Select(x => new TeamDto(x)).ToList();
    }

    public Guid LeagueId { get; set; }
    public List<TeamDto> Teams { get; set; }
}
