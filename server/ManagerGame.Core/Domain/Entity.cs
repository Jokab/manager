namespace ManagerGame.Core.Domain;

public abstract class Entity(Guid id)
{
    public Guid Id { get; init; } = id;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
