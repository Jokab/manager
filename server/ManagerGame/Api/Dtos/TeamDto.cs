using System.Text.Json.Serialization;

namespace ManagerGame.Api.Dtos;

public record TeamDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TeamDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public TeamDto(Team team)
    {
        Players = team.Players.Select(x => new PlayerDto(x.Player)).ToList();
        Id = team.Id;
        CreatedDate = team.CreatedDate;
        UpdatedDate = team.UpdatedDate;
        DeletedDate = team.DeletedDate;
        Name = team.Name.Name;
        ManagerId = team.ManagerId;
        LeagueId = team.LeagueId;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }
    public Guid ManagerId { get; set; }
    public List<PlayerDto> Players { get; set; }
    public Guid LeagueId { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
