namespace ManagerGame.Domain;

public abstract class Entity
{
    protected Entity(Guid id) => Id = id;

    public Guid Id { get; private init; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
