using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Domain.Interfaces;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<bool> IdentifierExistsAsync(string identifier, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
