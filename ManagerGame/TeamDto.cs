using System.Text.Json.Serialization;
using ManagerGame.Core.Domain;

namespace ManagerGame;

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
        Id = team.Id;
        CreatedDate = team.CreatedDate;
        UpdatedDate = team.UpdatedDate;
        DeletedDate = team.DeletedDate;
        Name = team.Name;
        ManagerId = team.ManagerId;
    }

    public Guid Id { get; set; }

    public TeamName Name { get; init; }
    public Guid ManagerId { get; init; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}