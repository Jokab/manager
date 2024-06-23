using System.ComponentModel.DataAnnotations;

namespace ManagerGame.Domain;

public class Entity
{
    protected Entity()
    {
        Id = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
}