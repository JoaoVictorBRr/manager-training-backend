using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class StudentMap : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Student");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.BirthDate).IsRequired();
        builder.Property(s => s.PaymentStatus).IsRequired(false).HasMaxLength(30);
        builder.Property(s => s.LastAccessDate).IsRequired(false);
        builder.Property(s => s.Observation).HasMaxLength(2000);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
