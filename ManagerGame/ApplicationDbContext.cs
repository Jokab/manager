using System.ComponentModel.DataAnnotations;
using ManagerGame.Domain;
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
        modelBuilder.Entity<Manager>()
            .HasMany(x => x.Teams)
            .WithOne()
            .HasForeignKey(x => x.ManagerId)
            .IsRequired();
    }
}

public abstract class Entity
{
	public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
}

public class Team(string name, Guid managerId) : Entity
{
	[Key]
	public Guid Id { get; private init; }
    public string Name { get; init; } = name;
    public Guid ManagerId { get; init; } = managerId;
}

public class Manager : Entity
{
	[Key]
	public Guid Id { get; private init; }
	public string Name { get; private init; }
	public string Email { get; private init; }
	public List<Team> Teams { get; init; } = [];

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
}
