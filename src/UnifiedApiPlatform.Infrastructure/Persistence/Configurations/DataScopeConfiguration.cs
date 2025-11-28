using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class DataScopeConfiguration : IEntityTypeConfiguration<DataScope>
{
    public void Configure(EntityTypeBuilder<DataScope> builder)
    {
        builder.ToTable("data_scopes");

        builder.HasKey(ds => ds.Id);

        builder.Property(ds => ds.ResourceType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ds => ds.ScopeValue)
            .HasMaxLength(2000);

        builder.Property(ds => ds.FilterExpression)
            .HasMaxLength(2000);

        builder.HasIndex(ds => ds.RoleId)
            .HasDatabaseName("ix_data_scopes_role_id");

        builder.HasIndex(ds => new { ds.RoleId, ds.ResourceType })
            .HasDatabaseName("ix_data_scopes_role_resource");
    }
}
