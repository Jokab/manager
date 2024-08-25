namespace ManagerGame.Domain;

public abstract class Entity
{
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
}