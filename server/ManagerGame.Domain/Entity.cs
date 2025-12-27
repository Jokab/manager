namespace ManagerGame.Domain;

public abstract class Entity
{
    protected Entity()
    {
    }

    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
