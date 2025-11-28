using NodaTime;

namespace UnifiedApiPlatform.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public string? CreatedBy { get; set; }
    public Instant CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public Instant? UpdatedAt { get; set; }
}
