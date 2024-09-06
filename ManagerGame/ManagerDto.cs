using System.Text.Json.Serialization;
using ManagerGame.Core.Domain;

namespace ManagerGame;

public record ManagerDto
{
    [JsonConstructor]
    public ManagerDto() { }

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