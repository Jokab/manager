using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ManagerGame;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Team> Teams { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSnakeCaseNamingConvention();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>().HasMany(x => x.Teams).WithOne().HasForeignKey(x => x.ManagerId).IsRequired();
    }
}

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

public class Team(string name, Guid managerId) : Entity
{
    public string Name { get; init; } = name;
    public Guid ManagerId { get; init; } = managerId;
}

public class Manager : Entity
{
    public Manager(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public void AddTeam(Team team)
    {
        if (Teams.Exists(x => x.Name == team.Name))
        {
            throw new InvalidOperationException("Team by that name already exists");
        }
        Teams.Add(team);
    }
    
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Team> Teams { get; init; } = [];
}