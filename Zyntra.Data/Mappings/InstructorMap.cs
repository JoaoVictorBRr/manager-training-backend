using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class InstructorMap : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructor");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Specialty).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Cref).IsRequired().HasMaxLength(20);
        builder.Property(i => i.Observation).HasMaxLength(2000);

        builder.HasIndex(i => i.Cref).IsUnique();

        builder.HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
