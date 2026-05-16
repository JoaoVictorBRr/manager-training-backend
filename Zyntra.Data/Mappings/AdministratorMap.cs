using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class AdministratorMap : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.ToTable("Administrator");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.AccessLevel).IsRequired().HasDefaultValue(1);
        builder.Property(a => a.Observation).HasMaxLength(2000);

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
