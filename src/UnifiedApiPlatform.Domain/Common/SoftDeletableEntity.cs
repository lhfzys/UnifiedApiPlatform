using NodaTime;

namespace UnifiedApiPlatform.Domain.Common;

public abstract class SoftDeletableEntity : AuditableEntity
{
    public bool IsDeleted { get; set; }
    public string? DeletedBy { get; set; }
    public Instant? DeletedAt { get; set; }
}
