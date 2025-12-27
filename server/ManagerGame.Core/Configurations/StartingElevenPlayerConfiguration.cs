using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class StartingElevenPlayerConfiguration : IEntityTypeConfiguration<StartingElevenPlayer>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<StartingElevenPlayer> builder)
    {
        builder.HasKey(x => x.Id);

        // Keep FK constraint names stable even if DbSet properties are removed from DbContext.
        builder.HasOne(x => x.Player)
            .WithMany(x => x.StartingElevenPlayers)
            .HasForeignKey(x => x.PlayerId)
            .IsRequired()
            .HasConstraintName("fk_starting_eleven_player_players_player_id");

        builder.HasOne(x => x.StartingEleven)
            .WithMany(x => x.SelectedPlayers)
            .HasForeignKey(x => x.StartingElevenId)
            .IsRequired()
            .HasConstraintName("fk_starting_eleven_player_starting_elevens_starting_eleven_id");
    }
}
