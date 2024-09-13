using System.Text.Json.Serialization;
using ManagerGame.Core.Domain;

namespace ManagerGame.Api.Dtos;

public record ManagerDto
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ManagerDto()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ManagerDto(Manager manager)
    {
        Id = manager.Id;
        CreatedDate = manager.CreatedDate;
        UpdatedDate = manager.CreatedDate;
        DeletedDate = manager.DeletedDate;
        Name = manager.Name;
        Email = manager.Email;
    }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public ManagerName Name { get; set; }
    public Email Email { get; set; }
    public List<Team> Teams { get; init; } = [];
}
