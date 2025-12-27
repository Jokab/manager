using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public AggregateRepository<Draft, Guid> Drafts2 => new(
        Set<Draft>(),
        Set<Draft>()
            .Include(x => x.Participants)
            .Include(x => x.Picks),
        draft => draft.Id);

    public AggregateRepository<League, Guid> Leagues2 => new(
        Set<League>(),
        Set<League>()
            .Include(x => x.Teams)
            .Include(x => x.Settings)
            .Include(x => x.Drafts),
        league => league.Id);

    public AggregateRepository<Manager, Guid> Managers2 => new(
        Set<Manager>(),
        Set<Manager>().Include(x => x.Teams),
        manager => manager.Id);

    public AggregateRepository<Team, Guid> Teams2 => new(
        Set<Team>(),
        Set<Team>()
            .Include(x => x.Players)
            .ThenInclude(x => x.Player)
            .Include(x => x.League)
            .Include(x => x.StartingElevens)
            .ThenInclude(x => x.SelectedPlayers),
        team => team.Id);

    public AggregateRepository<Player, Guid> Players2 => new(
        Set<Player>(),
        Set<Player>()
            .Include(x => x.TeamPlayers)
            .Include(x => x.StartingElevenPlayers),
        player => player.Id);

    public AggregateRepository<TeamPlayer, Guid> TeamPlayers2 => new(
        Set<TeamPlayer>(),
        Set<TeamPlayer>().Include(x => x.Team).Include(x => x.Player),
        teamPlayer => teamPlayer.Id);

    public AggregateRepository<MatchResult, Guid> MatchResults2 => new(
        Set<MatchResult>(),
        Set<MatchResult>().Include(x => x.League).Include(x => x.MatchEvents),
        match => match.Id);

    public AggregateRepository<MatchEvent, Guid> MatchEvents2 => new(
        Set<MatchEvent>(),
        Set<MatchEvent>().Include(x => x.Match).Include(x => x.Player),
        matchEvent => matchEvent.Id);

    public AggregateRepository<StartingEleven, Guid> StartingElevens2 => new(
        Set<StartingEleven>(),
        Set<StartingEleven>().Include(x => x.Team).Include(x => x.SelectedPlayers),
        startingEleven => startingEleven.Id);

    public AggregateRepository<LeagueSettings, Guid> LeagueSettings2 => new(
        Set<LeagueSettings>(),
        Set<LeagueSettings>().Include(x => x.League),
        leagueSettings => leagueSettings.Id);

    public DbSet<Manager> Managers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Draft> Drafts { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<MatchResult> MatchResults { get; set; }
    public DbSet<MatchEvent> MatchEvents { get; set; }
    public DbSet<StartingEleven> StartingElevens { get; set; }
    public DbSet<LeagueSettings> LeagueSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>().Navigation(x => x.Teams).AutoInclude();
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
            .WithOne(tp => tp.Team)
            .HasForeignKey(tp => tp.TeamId)
            .IsRequired();
        modelBuilder.Entity<Team>().Navigation(x => x.League).AutoInclude();
        modelBuilder.Entity<Team>()
            .HasMany(x => x.StartingElevens)
            .WithOne(x => x.Team)
            .HasForeignKey(x => x.TeamId)
            .IsRequired();

        modelBuilder.Entity<Player>().HasKey(x => x.Id);
        modelBuilder.Entity<Player>()
            .Property(x => x.Name)
            .HasConversion(x => x.Name, x => new PlayerName(x));
        modelBuilder.Entity<Player>()
            .Property(x => x.Country)
            .HasConversion(x => x.Country.ToString(), x => new CountryRec(Enum.Parse<Country>(x)));
        modelBuilder.Entity<Player>()
            .HasMany(x => x.TeamPlayers)
            .WithOne(x => x.Player)
            .HasForeignKey(x => x.PlayerId)
            .IsRequired();
        modelBuilder.Entity<Player>()
            .HasMany(x => x.StartingElevenPlayers)
            .WithOne(x => x.Player)
            .HasForeignKey(x => x.PlayerId)
            .IsRequired();

        modelBuilder.Entity<Draft>().HasKey(x => x.Id);
        modelBuilder.Entity<Draft>().OwnsOne(typeof(DraftOrder),
            "DraftOrder",
            x =>
            {
                x.Property("_current").HasColumnName("draftOrderCurrent");
                x.Property("_previous").HasColumnName("draftOrderPrevious");
            });
        modelBuilder.Entity<Draft>().Property(x => x.State)
            .HasConversion<string>(x => x.ToString(), x => Enum.Parse<DraftState>(x));
        // Draft is an aggregate; avoid auto-including the entire League graph.

        modelBuilder.Entity<DraftParticipant>().HasKey(x => x.Id);
        modelBuilder.Entity<DraftParticipant>()
            .HasIndex(x => new { x.DraftId, x.Seat })
            .IsUnique();
        modelBuilder.Entity<DraftParticipant>()
            .HasIndex(x => new { x.DraftId, x.TeamId })
            .IsUnique();
        modelBuilder.Entity<DraftParticipant>()
            .HasOne<Draft>()
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.DraftId)
            .IsRequired();

        modelBuilder.Entity<DraftPick>().HasKey(x => x.Id);
        modelBuilder.Entity<DraftPick>()
            .HasIndex(x => new { x.DraftId, x.PickNumber })
            .IsUnique();
        modelBuilder.Entity<DraftPick>()
            .HasIndex(x => new { x.DraftId, x.PlayerId })
            .IsUnique();
        modelBuilder.Entity<DraftPick>()
            .HasOne<Draft>()
            .WithMany(x => x.Picks)
            .HasForeignKey(x => x.DraftId)
            .IsRequired();

        modelBuilder.Entity<League>().HasKey(x => x.Id);
        modelBuilder.Entity<League>().Navigation(x => x.Teams).AutoInclude();
        modelBuilder.Entity<League>()
            .HasMany(x => x.Drafts)
            .WithOne(x => x.League).HasForeignKey(x => x.LeagueId)
            .IsRequired();
        modelBuilder.Entity<League>()
            .HasMany(x => x.Teams)
            .WithOne(x => x.League)
            .HasForeignKey(x => x.LeagueId)
            .IsRequired();
        modelBuilder.Entity<League>()
            .HasMany(x => x.MatchResults)
            .WithOne(x => x.League)
            .HasForeignKey(x => x.LeagueId)
            .IsRequired();
        modelBuilder.Entity<League>()
            .HasOne(x => x.Settings)
            .WithOne(x => x.League)
            .HasForeignKey<LeagueSettings>(x => x.LeagueId)
            .IsRequired();

        modelBuilder.Entity<TeamPlayer>().HasKey(x => x.Id);
        modelBuilder.Entity<TeamPlayer>().Navigation(x => x.Team).AutoInclude();
        modelBuilder.Entity<TeamPlayer>().Navigation(x => x.Player).AutoInclude();
        modelBuilder.Entity<TeamPlayer>()
            .HasIndex(x => new { x.LeagueId, x.PlayerId })
            .IsUnique();
        modelBuilder.Entity<TeamPlayer>()
            .HasOne<League>()
            .WithMany()
            .HasForeignKey(x => x.LeagueId)
            .IsRequired();
        modelBuilder.Entity<TeamPlayer>()
            .HasOne(x => x.Player)
            .WithMany(x => x.TeamPlayers)
            .HasForeignKey(x => x.PlayerId)
            .IsRequired();

        modelBuilder.Entity<StartingEleven>().HasKey(x => x.Id);
        modelBuilder.Entity<StartingEleven>().Navigation(x => x.Team).AutoInclude();
        modelBuilder.Entity<StartingEleven>().Navigation(x => x.SelectedPlayers).AutoInclude();

        modelBuilder.Entity<StartingElevenPlayer>().HasKey(x => x.Id);
        modelBuilder.Entity<StartingElevenPlayer>().Navigation(x => x.StartingEleven).AutoInclude();
        modelBuilder.Entity<StartingElevenPlayer>().Navigation(x => x.Player).AutoInclude();

        modelBuilder.Entity<MatchResult>().HasKey(x => x.Id);
        modelBuilder.Entity<MatchResult>().Navigation(x => x.League).AutoInclude();
        modelBuilder.Entity<MatchResult>().Navigation(x => x.MatchEvents).AutoInclude();

        modelBuilder.Entity<MatchEvent>().HasKey(x => x.Id);
        modelBuilder.Entity<MatchEvent>().Navigation(x => x.Match).AutoInclude();
        modelBuilder.Entity<MatchEvent>().Navigation(x => x.Player).AutoInclude();
        modelBuilder.Entity<MatchEvent>()
            .Property(x => x.EventType)
            .HasConversion<string>(x => x.ToString(), x => Enum.Parse<MatchEventType>(x));

        modelBuilder.Entity<LeagueSettings>().HasKey(x => x.Id);
        modelBuilder.Entity<LeagueSettings>().Navigation(x => x.League).AutoInclude();
    }
}
