using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class WaitListMap : IEntityTypeConfiguration<WaitList>
{
    public void Configure(EntityTypeBuilder<WaitList> builder)
    {
        builder.ToTable("WaitList");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Position).IsRequired();
        builder.Property(w => w.InclusionDateTime).IsRequired();
        builder.Property(w => w.Observation).HasMaxLength(2000);

        builder.HasOne(w => w.Student)
            .WithMany()
            .HasForeignKey(w => w.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Class)
            .WithMany(c => c.WaitLists)
            .HasForeignKey(w => w.ClassId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
