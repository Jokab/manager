using Microsoft.EntityFrameworkCore;

namespace ManagerGame.Core.Configurations;

public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Manager> builder)
    {
        builder.HasMany(x => x.Teams)
            .WithOne()
            .HasForeignKey(x => x.ManagerId)
            .IsRequired();
        builder.Property(x => x.Email)
            .HasConversion(x => x.EmailAddress, x => new Email(x));
        builder.Property(x => x.Name).HasConversion(x => x.Name, x => new ManagerName(x));
    }
}
