using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class PhysicalAssessmentMap : IEntityTypeConfiguration<PhysicalAssessment>
{
    public void Configure(EntityTypeBuilder<PhysicalAssessment> builder)
    {
        builder.ToTable("PhysicalAssessment");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.AssessmentDate).IsRequired();
        builder.Property(p => p.Weight).IsRequired().HasPrecision(6, 2);
        builder.Property(p => p.Height).IsRequired().HasPrecision(5, 2);
        builder.Property(p => p.Bmi).IsRequired().HasPrecision(5, 2);
        builder.Property(p => p.BodyFatPercentage).HasPrecision(5, 2);
        builder.Property(p => p.Measurements).HasMaxLength(1000);
        builder.Property(p => p.Notes).HasMaxLength(2000);
        builder.Property(p => p.Observation).HasMaxLength(2000);

        builder.HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
