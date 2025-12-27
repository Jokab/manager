using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class DraftConfiguration : IEntityTypeConfiguration<Draft>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Draft> builder)
    {
        builder.HasKey(x => x.Id);
        builder.OwnsOne<DraftOrder>("DraftOrder", x =>
            {
                x.Property("_current").HasColumnName("draftOrderCurrent");
                x.Property("_previous").HasColumnName("draftOrderPrevious");
            });
        builder.Property(x => x.State)
            .HasConversion<string>(x => x.ToString(), x => Enum.Parse<DraftState>(x));
        // Draft is an aggregate; avoid auto-including the entire League graph.
    }
}
