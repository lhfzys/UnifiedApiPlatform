using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");

        // 主键
        builder.HasKey(o => o.Id);

        // ParentId
        builder.Property(o => o.ParentId);

        // Code（唯一）
        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(50);

        // Name
        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        // FullName
        builder.Property(o => o.FullName)
            .HasMaxLength(500);

        // Type
        builder.Property(o => o.Type)
            .IsRequired()
            .HasMaxLength(50);

        // Description
        builder.Property(o => o.Description)
            .HasMaxLength(1000);

        // ManagerId
        builder.Property(o => o.ManagerId);

        // Level
        builder.Property(o => o.Level)
            .IsRequired()
            .HasDefaultValue(1);

        // Path
        builder.Property(o => o.Path)
            .IsRequired()
            .HasMaxLength(1000);

        // SortOrder
        builder.Property(o => o.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // IsActive
        builder.Property(o => o.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // TenantId
        builder.Property(o => o.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        // 审计字段
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.CreatedBy).HasMaxLength(50);
        builder.Property(o => o.UpdatedAt);
        builder.Property(o => o.UpdatedBy).HasMaxLength(50);

        // 软删除
        builder.Property(o => o.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(o => o.DeletedAt);
        builder.Property(o => o.DeletedBy).HasMaxLength(50);

        // 行版本
        builder.Property(o => o.RowVersion).IsRowVersion();

        // 索引
        builder.HasIndex(o => new { o.TenantId, o.Code })
            .IsUnique()
            .HasDatabaseName("ix_organizations_tenant_code");

        builder.HasIndex(o => o.ParentId)
            .HasDatabaseName("ix_organizations_parent_id");

        builder.HasIndex(o => o.Path)
            .HasDatabaseName("ix_organizations_path");

        builder.HasIndex(o => o.ManagerId)
            .HasDatabaseName("ix_organizations_manager_id");

        // 关系配置

        // 父子组织关系（自引用）
        builder.HasOne(o => o.Parent)
            .WithMany(o => o.Children)
            .HasForeignKey(o => o.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // 组织负责人
        builder.HasOne(o => o.Manager)
            .WithMany()
            .HasForeignKey(o => o.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        // 组织成员（User 中的 OrganizationId）
        // 在 UserConfiguration 中配置

        // 全局查询过滤器（软删除）
        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}
