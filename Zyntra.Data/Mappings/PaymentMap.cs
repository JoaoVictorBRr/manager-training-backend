using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class PaymentMap : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payment");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Amount).IsRequired().HasPrecision(10, 2);
        builder.Property(p => p.DueDate).IsRequired();
        builder.Property(p => p.PaymentDate);
        builder.Property(p => p.PaymentStatus).IsRequired();
        builder.Property(p => p.PaymentMethod).HasMaxLength(50);
        builder.Property(p => p.Observation).HasMaxLength(2000);

        builder.HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
