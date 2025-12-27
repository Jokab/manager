using System.Text.Json.Serialization;

namespace ManagerGame.Api.Dtos;

public class CreateLeagueDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public CreateLeagueDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public CreateLeagueDto(League league)
    {
        Id = league.Id;
        Teams = league.Teams.Select(x => new TeamDto(x)).ToList();
    }

    public Guid Id { get; set; }
    public List<TeamDto> Teams { get; set; }
}
