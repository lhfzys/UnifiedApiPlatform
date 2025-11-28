using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Path)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        // 唯一约束
        builder.HasIndex(o => new { o.TenantId, o.Code })
            .IsUnique()
            .HasFilter("is_deleted = false")
            .HasDatabaseName("ix_organizations_tenant_code");

        // 索引
        builder.HasIndex(o => o.TenantId)
            .HasDatabaseName("ix_organizations_tenant_id");

        builder.HasIndex(o => o.ParentId)
            .HasDatabaseName("ix_organizations_parent_id");

        builder.HasIndex(o => o.Path)
            .HasDatabaseName("ix_organizations_path");

        // 自引用关系
        builder.HasOne(o => o.Parent)
            .WithMany(o => o.Children)
            .HasForeignKey(o => o.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Manager)
            .WithMany()
            .HasForeignKey(o => o.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
