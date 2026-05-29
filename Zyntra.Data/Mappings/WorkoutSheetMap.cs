using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class WorkoutSheetMap : IEntityTypeConfiguration<WorkoutSheet>
{
    public void Configure(EntityTypeBuilder<WorkoutSheet> builder)
    {
        builder.ToTable("WorkoutSheet");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.StartDate).IsRequired();
        builder.Property(w => w.EndDate).IsRequired(false);
        builder.Property(w => w.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(w => w.Notes).HasMaxLength(2000);
        builder.Property(w => w.Observation).HasMaxLength(2000);

        builder.HasOne(w => w.Student)
            .WithMany()
            .HasForeignKey(w => w.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Instructor)
            .WithMany()
            .HasForeignKey(w => w.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
