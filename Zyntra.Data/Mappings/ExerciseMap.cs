using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class ExerciseMap : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercise");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.MuscleGroup).IsRequired().HasMaxLength(80);
        builder.Property(e => e.Sets).IsRequired();
        builder.Property(e => e.Repetitions).IsRequired();
        builder.Property(e => e.SuggestedLoad).HasPrecision(10, 2);
        builder.Property(e => e.VideoUrl).HasMaxLength(500);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.Observation).HasMaxLength(2000);

        builder.HasOne(e => e.WorkoutSheet)
            .WithMany(w => w.Exercises)
            .HasForeignKey(e => e.WorkoutSheetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
