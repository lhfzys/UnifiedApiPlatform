using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Interceptors;

/// <summary>
/// 领域事件拦截器
/// </summary>
public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;

    public DomainEventInterceptor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await PublishDomainEventsAsync(eventData.Context);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishDomainEventsAsync(DbContext? context)
    {
        if (context == null) return;

        var domainEvents = context.ChangeTracker.Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.DomainEvents.Any())
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        // 清除领域事件
        context.ChangeTracker.Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .ToList()
            .ForEach(entity => entity.ClearDomainEvents());

        // 发布领域事件
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}
