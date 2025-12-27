using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class StartingElevenPlayerConfiguration : IEntityTypeConfiguration<StartingElevenPlayer>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<StartingElevenPlayer> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
