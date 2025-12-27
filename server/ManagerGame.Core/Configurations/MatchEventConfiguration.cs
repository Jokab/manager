using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class MatchEventConfiguration : IEntityTypeConfiguration<MatchEvent>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<MatchEvent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType)
            .HasConversion<string>(x => x.ToString(), x => Enum.Parse<MatchEventType>(x));
    }
}
