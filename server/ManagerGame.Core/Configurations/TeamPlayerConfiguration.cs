using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class TeamPlayerConfiguration : IEntityTypeConfiguration<TeamPlayer>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TeamPlayer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.LeagueId, x.PlayerId })
            .IsUnique();
        builder.HasOne<League>()
            .WithMany()
            .HasForeignKey(x => x.LeagueId)
            .IsRequired();
        builder.HasOne(x => x.Player)
            .WithMany(x => x.TeamPlayers)
            .HasForeignKey(x => x.PlayerId)
            .IsRequired();
    }
}
