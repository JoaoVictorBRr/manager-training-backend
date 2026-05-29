using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class CheckInMap : IEntityTypeConfiguration<CheckIn>
{
    public void Configure(EntityTypeBuilder<CheckIn> builder)
    {
        builder.ToTable("CheckIn");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.DateTime).IsRequired();
        builder.Property(c => c.Unit).IsRequired().HasMaxLength(80);
        builder.Property(c => c.AccessType).IsRequired();
        builder.Property(c => c.ValidationStatus).IsRequired();
        builder.Property(c => c.Observation).IsRequired(false).HasMaxLength(2000);

        builder.HasOne(c => c.Student)
            .WithMany()
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
