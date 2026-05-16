using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class ChatMessageMap : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessage");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Message).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.MessageDateTime).IsRequired();
        builder.Property(c => c.IsRead).IsRequired().HasDefaultValue(false);
        builder.Property(c => c.Observation).HasMaxLength(2000);

        builder.HasOne(c => c.Student)
            .WithMany()
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Instructor)
            .WithMany()
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
