using NodaTime;

namespace UnifiedApiPlatform.Domain.Common;

public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Instant OccurredOn { get; }
    public string EventType => GetType().Name;

    protected DomainEvent()
    {
        OccurredOn = SystemClock.Instance.GetCurrentInstant();
    }
}
