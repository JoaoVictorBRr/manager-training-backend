using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class ClassMap : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("Class");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Modality).IsRequired().HasMaxLength(80);
        builder.Property(c => c.DateTime).IsRequired();
        builder.Property(c => c.MaxCapacity).IsRequired();
        builder.Property(c => c.AvailableSlots).IsRequired();
        builder.Property(c => c.Unit).IsRequired().HasMaxLength(80);
        builder.Property(c => c.Observation).HasMaxLength(2000);

        builder.HasOne(c => c.Instructor)
            .WithMany()
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
