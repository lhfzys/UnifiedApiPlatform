using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Domain.Interfaces;

public interface IUserRepository: IRepository<User>
{
    Task<User?> GetByEmailAsync(string tenantId, string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUserNameAsync(string tenantId, string userName, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string tenantId, string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<List<User>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
