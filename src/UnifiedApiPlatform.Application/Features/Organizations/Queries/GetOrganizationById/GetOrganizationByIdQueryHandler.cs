using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationById;

public class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, Result<OrganizationDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetOrganizationByIdQueryHandler> _logger;

    public GetOrganizationByIdQueryHandler(
        IApplicationDbContext context,
        ILogger<GetOrganizationByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<OrganizationDetailDto>> Handle(
        GetOrganizationByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _context.Organizations
                .AsNoTracking()
                .Include(o => o.Parent)
                .FirstOrDefaultAsync(o => o.Id == request.OrganizationId && o.TenantId == request.CurrentTenantId,
                    cancellationToken);

            if (organization == null)
            {
                _logger.LogWarning("组织不存在: {OrganizationId}", request.OrganizationId);
                return Result.Fail<OrganizationDetailDto>(ErrorCodes.OrganizationNotFound);
            }

            var dto = new OrganizationDetailDto
            {
                Id = organization.Id,
                ParentId = organization.ParentId,
                ParentName = organization.Parent?.Name,
                Code = organization.Code,
                Name = organization.Name,
                FullName = organization.FullName,
                Description = organization.Description,
                SortOrder = organization.SortOrder,
                IsActive = organization.IsActive,
                UserCount =
                    await _context.Users.CountAsync(u => u.OrganizationId == organization.Id, cancellationToken),
                ChildCount =
                    await _context.Organizations.CountAsync(o => o.ParentId == organization.Id, cancellationToken),
                CreatedAt = organization.CreatedAt.ToDateTimeUtc(),
                CreatedBy = organization.CreatedBy,
                UpdatedAt = organization.UpdatedAt?.ToDateTimeUtc(),
                UpdatedBy = organization.UpdatedBy,
                RowVersion = organization.RowVersion != null
                    ? Convert.ToBase64String(organization.RowVersion)
                    : null
            };

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取组织详情失败: {OrganizationId}", request.OrganizationId);
            return Result.Fail<OrganizationDetailDto>("获取组织详情失败");
        }
    }
}
