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