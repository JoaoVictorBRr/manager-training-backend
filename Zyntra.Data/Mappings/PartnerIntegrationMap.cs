using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Mappings;

public class PartnerIntegrationMap : IEntityTypeConfiguration<PartnerIntegration>
{
    public void Configure(EntityTypeBuilder<PartnerIntegration> builder)
    {
        builder.ToTable("PartnerIntegration");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.PartnerName).IsRequired().HasMaxLength(80);
        builder.Property(p => p.IntegrationType).IsRequired();
        builder.Property(p => p.Token).IsRequired().HasMaxLength(500);
        builder.Property(p => p.ValidationStatus).IsRequired();
        builder.Property(p => p.Observation).IsRequired(false).HasMaxLength(2000);

        builder.HasIndex(p => p.Token).IsUnique();
    }
}
