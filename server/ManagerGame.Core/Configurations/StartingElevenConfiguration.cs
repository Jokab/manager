using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class StartingElevenConfiguration : IEntityTypeConfiguration<StartingEleven>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<StartingEleven> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
