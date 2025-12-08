using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";
            // 查询用户
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == request.UserId && u.TenantId == tenantId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("用户不存在: {UserId}", request.UserId);
                return Result.Fail(ErrorCodes.UserNotFound);
            }

            user.IsDeleted = true;
            user.DeletedAt = _clock.GetCurrentInstant();
            user.DeletedBy = _currentUser.UserId;

            user.Deactivate();

            foreach (var token in user.RefreshTokens)
            {
                _context.RefreshTokens.Remove(token);
            }

            // 软删除用户角色关联
            foreach (var userRole in user.UserRoles.ToList())
            {
                _context.UserRoles.Remove(userRole);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "用户删除成功: {UserId}, 用户名: {UserName}, 操作人: {OperatorId}, IP: {IpAddress}",
                user.Id,
                user.UserName,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除用户失败: {UserId}", request.UserId);
            return Result.Fail("删除用户失败");
        }
    }
}
