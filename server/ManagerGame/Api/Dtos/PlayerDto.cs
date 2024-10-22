using System.Text.Json.Serialization;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Dtos;

internal record PlayerDto
{
    public PlayerDto(Player player)
    {
        Id = player.Id;
        CreatedDate = player.CreatedDate;
        UpdatedDate = player.CreatedDate;
        DeletedDate = player.DeletedDate;
        Country = player.Country.Country.ToString();
        Name = player.Name.Name;
        TeamId = player.TeamId;
        Position = player.Position.ToString();
        IsSigned = player.IsSigned;
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
    public Guid? TeamId { get; set; }
    public string Name { get; init; }
    public string Position { get; init; }
    public string Country { get; init; }
    public bool IsSigned { get; set; }
}
