using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Password).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Salt).IsRequired().HasMaxLength(255);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(80);
        builder.Property(u => u.Observation).HasMaxLength(2000);
        builder.Property(u => u.CellphoneNumber).IsRequired(false).HasMaxLength(15);
        builder.Property(u => u.Role).IsRequired().HasDefaultValueSql("1");
        builder.Property(u => u.Token).IsRequired(false).HasMaxLength(1000);
        builder.Property(u => u.ExpirationDateToken).IsRequired(false);
    }
}