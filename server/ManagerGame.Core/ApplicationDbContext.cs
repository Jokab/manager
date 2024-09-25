using ManagerGame.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }

    public DbSet<Draft> Drafts { get; set; }
    // public DbSet<DoubledPeakTraversalDraftOrder> DoubledPeakTraversalDraftOrders { get; set; }

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
        modelBuilder.Entity<Manager>()
            .Property(x => x.Email)
            .HasConversion(x => x.EmailAddress, x => new Email(x));
        modelBuilder.Entity<Manager>()
            .Property(x => x.Name).HasConversion(x => x.Name, x => new ManagerName(x));

        modelBuilder.Entity<Team>()
            .Property(x => x.Name)
            .HasConversion(x => x.Name, x => new TeamName(x));
        modelBuilder.Entity<Team>()
            .HasMany(x => x.Players)
            .WithOne()
            .HasForeignKey(x => x.TeamId);

        modelBuilder.Entity<Player>().HasKey(x => x.Id);
        modelBuilder.Entity<Player>()
            .Property(x => x.Name)
            .HasConversion(x => x.Name, x => new PlayerName(x));
        modelBuilder.Entity<Player>()
            .Property(x => x.Country)
            .HasConversion(x => x.Country.ToString(), x => new CountryRec(Enum.Parse<Country>(x)));

        modelBuilder.Entity<Draft>().HasKey(x => x.Id);
        modelBuilder.Entity<Draft>().OwnsOne(typeof(DraftOrder),
            "DraftOrder",
            x =>
            {
                x.Ignore("_teams");
                x.Property("_current").HasColumnName("DraftOrderCurrent");
                x.Property("_previous").HasColumnName("DraftOrderPrevious");
            });
        modelBuilder.Entity<Draft>().Ignore(x => x.Teams);
        modelBuilder.Entity<Draft>().Property(x => x.State)
            .HasConversion<string>(x => x.ToString(), x => Enum.Parse<State>(x));

        modelBuilder.Entity<League>().HasKey(x => x.Id);
        modelBuilder.Entity<League>()
            .HasMany<Draft>(x => x.Drafts)
            .WithOne().HasForeignKey(x => x.LeagueId)
            .IsRequired();
        modelBuilder.Entity<League>()
            .HasMany<Team>(x => x.Teams)
            .WithOne()
            .HasForeignKey(x => x.LeagueId)
            .IsRequired(false);
    }
}