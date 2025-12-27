using System.Text.Json.Serialization;

namespace ManagerGame.Api.Dtos;

public record PlayerDto
{
    public PlayerDto(Player player)
    {
        Id = player.Id;
        CreatedDate = player.CreatedDate;
        UpdatedDate = player.CreatedDate;
        DeletedDate = player.DeletedDate;
        Country = player.Country.Country.ToString();
        Name = player.Name.Name;
        Position = player.Position.ToString();
    }

    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public PlayerDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public DateTime? DeletedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid Id { get; set; }
    public string Name { get; init; }
    public string Position { get; init; }
    public string Country { get; init; }
}
