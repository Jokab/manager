using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class MatchEventConfiguration : IEntityTypeConfiguration<MatchEvent>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<MatchEvent> builder)
    {
        // Keep table name stable even if DbSet properties are removed from DbContext.
        builder.ToTable("match_events");
        builder.HasKey(x => x.Id);

        // Keep FK constraint names stable even if DbSet properties are removed from DbContext.
        builder.HasOne(x => x.Match)
            .WithMany(x => x.MatchEvents)
            .HasForeignKey(x => x.MatchId)
            .IsRequired()
            .HasConstraintName("fk_match_events_match_results_match_id");

        builder.HasOne(x => x.Player)
            .WithMany()
            .HasForeignKey(x => x.PlayerId)
            .IsRequired()
            .HasConstraintName("fk_match_events_players_player_id");

        builder.Property(x => x.EventType)
            .HasConversion<string>(x => x.ToString(), x => Enum.Parse<MatchEventType>(x));
    }
}
