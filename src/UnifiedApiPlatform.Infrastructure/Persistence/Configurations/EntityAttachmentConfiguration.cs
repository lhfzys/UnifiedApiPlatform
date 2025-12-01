using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Configurations;

public class EntityAttachmentConfiguration : IEntityTypeConfiguration<EntityAttachment>
{
    public void Configure(EntityTypeBuilder<EntityAttachment> builder)
    {
        builder.ToTable("entity_attachments");

        builder.HasKey(ea => ea.Id);

        builder.Property(ea => ea.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ea => ea.EntityId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ea => ea.AttachmentType)
            .HasMaxLength(50);

        builder.Property(ea => ea.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        // 索引
        builder.HasIndex(ea => new { ea.EntityType, ea.EntityId })
            .HasDatabaseName("ix_entity_attachments_entity");

        builder.HasIndex(ea => new { ea.EntityType, ea.EntityId, ea.AttachmentType })
            .HasDatabaseName("ix_entity_attachments_entity_type");

        // 关系配置
        builder.HasOne(ea => ea.File)
            .WithMany(f => f.EntityAttachments)
            .HasForeignKey(ea => ea.FileId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
