using ManagerGame.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>()
            .HasMany(x => x.Teams)
            .WithOne()
            .HasForeignKey(x => x.ManagerId)
            .IsRequired();

        modelBuilder.Entity<Manager>().Property(x => x.Email).HasConversion(x => x.EmailAddress, x => new Email(x));
        modelBuilder.Entity<Manager>().Property(x => x.Name).HasConversion(x => x.Name, x => new ManagerName(x));

        modelBuilder.Entity<Team>().Property(x => x.Name).HasConversion(x => x.Name, x => new TeamName(x));
    }
}