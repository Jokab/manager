using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .HasConversion(x => x.Name, x => new PlayerName(x));
        builder.Property(x => x.Country)
            .HasConversion(x => x.Country.ToString(), x => new CountryRec(Enum.Parse<Country>(x)));
        builder.HasMany(x => x.TeamPlayers)
            .WithOne(x => x.Player)
            .HasForeignKey(x => x.PlayerId)
            .IsRequired();
        builder.HasMany(x => x.StartingElevenPlayers)
            .WithOne(x => x.Player)
            .HasForeignKey(x => x.PlayerId)
            .IsRequired();
    }
}
