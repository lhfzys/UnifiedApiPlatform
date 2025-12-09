using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.AssignUsersToOrganization;

public class AssignUsersToOrganizationCommandHandler : IRequestHandler<AssignUsersToOrganizationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<AssignUsersToOrganizationCommandHandler> _logger;

    public AssignUsersToOrganizationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<AssignUsersToOrganizationCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result> Handle(AssignUsersToOrganizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";

            // 验证组织存在
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == request.OrganizationId && o.TenantId == tenantId,
                    cancellationToken);

            if (organization == null)
            {
                _logger.LogWarning("组织不存在: {OrganizationId}", request.OrganizationId);
                return Result.Fail(ErrorCodes.OrganizationNotFound);
            }

            // 查询用户
            var users = await _context.Users
                .Where(u => request.UserIds.Contains(u.Id) && u.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            if (users.Count != request.UserIds.Count)
            {
                _logger.LogWarning("部分用户不存在");
                return Result.Fail(ErrorCodes.UserNotFound);
            }

            // 更新用户的组织
            var now = _clock.GetCurrentInstant();
            foreach (var user in users)
            {
                user.OrganizationId = request.OrganizationId;
                user.UpdatedAt = now;
                user.UpdatedBy = _currentUser.UserId;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "批量分配用户到组织成功: {OrganizationId}, 用户数: {Count}, 操作人: {OperatorId}, IP: {IpAddress}",
                request.OrganizationId,
                users.Count,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量分配用户到组织失败: {OrganizationId}", request.OrganizationId);
            return Result.Fail("批量分配用户到组织失败");
        }
    }
}
