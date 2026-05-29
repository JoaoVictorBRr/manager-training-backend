using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class AiChatMessageMap : IEntityTypeConfiguration<AiChatMessage>
{
    public void Configure(EntityTypeBuilder<AiChatMessage> builder)
    {
        builder.ToTable("AiChatMessage");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Role).IsRequired().HasMaxLength(20);
        builder.Property(c => c.Content).IsRequired().HasMaxLength(8000);
        builder.Property(c => c.ActionJson).IsRequired(false).HasMaxLength(4000);
        builder.Property(c => c.ActionStatus).IsRequired().HasMaxLength(20).HasDefaultValue("none");
        builder.Property(c => c.Observation).IsRequired(false).HasMaxLength(2000);

        builder.HasOne(c => c.Student)
            .WithMany()
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
