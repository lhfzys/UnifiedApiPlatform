using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<DeleteOrganizationCommandHandler> _logger;

    public DeleteOrganizationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<DeleteOrganizationCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";

            // 查询组织
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == request.OrganizationId && o.TenantId == tenantId,
                    cancellationToken);

            if (organization == null)
            {
                _logger.LogWarning("组织不存在: {OrganizationId}", request.OrganizationId);
                return Result.Fail(ErrorCodes.OrganizationNotFound);
            }

            // 软删除
            organization.IsDeleted = true;
            organization.DeletedAt = _clock.GetCurrentInstant();
            organization.DeletedBy = _currentUser.UserId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "组织删除成功: {OrganizationId}, 名称: {Name}, 操作人: {OperatorId}, IP: {IpAddress}",
                organization.Id,
                organization.Name,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除组织失败: {OrganizationId}", request.OrganizationId);
            return Result.Fail("删除组织失败");
        }
    }
}
