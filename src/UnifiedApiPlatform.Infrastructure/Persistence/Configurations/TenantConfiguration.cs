using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Identifier)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.ContactName)
            .HasMaxLength(100);

        builder.Property(t => t.ContactEmail)
            .HasMaxLength(255);

        builder.Property(t => t.ContactPhone)
            .HasMaxLength(50);

        // 唯一约束
        builder.HasIndex(t => t.Identifier)
            .IsUnique()
            .HasDatabaseName("ix_tenants_identifier");

        // 一对多关系
        builder.HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .HasPrincipalKey(t => t.Identifier)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
