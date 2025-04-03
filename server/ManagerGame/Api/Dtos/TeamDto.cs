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
        Players = [.. team.Players.Select(x => x.Player)];
        Id = team.Id;
        CreatedDate = team.CreatedDate;
        UpdatedDate = team.UpdatedDate;
        DeletedDate = team.DeletedDate;
        Name = team.Name.Name;
        ManagerId = team.ManagerId;
        League = team.League;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }
    public Guid ManagerId { get; set; }
    public List<Player> Players { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public League? League { get; set; }
}
