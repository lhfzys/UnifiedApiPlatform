using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";
            var targetUserId = request.UserId ?? Guid.Parse(_currentUser.UserId!);
            var isChangingOwnPassword = targetUserId.ToString() == _currentUser.UserId;

            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == targetUserId && u.TenantId == tenantId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("用户不存在: {UserId}", targetUserId);
                return Result.Fail(ErrorCodes.UserNotFound);
            }

            if (isChangingOwnPassword)
            {
                if (string.IsNullOrEmpty(request.OldPassword))
                {
                    return Result.Fail("原密码不能为空");
                }

                if (!_passwordHasher.Verify(request.OldPassword, user.PasswordHash))
                {
                    _logger.LogWarning("原密码错误: {UserId}", targetUserId);
                    return Result.Fail(ErrorCodes.UserOldPasswordIncorrect);
                }
            }
            else
            {
                _logger.LogInformation("管理员重置用户密码: 操作人 {OperatorId}, 目标用户 {TargetUserId}",
                    _currentUser.UserId, targetUserId);
            }

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

            user.PasswordChangedAt = _clock.GetCurrentInstant();
            user.UpdatedAt = _clock.GetCurrentInstant();
            user.UpdatedBy = _currentUser.UserId;
            user.PasswordChangedAt = _clock.GetCurrentInstant();

            foreach (var token in user.RefreshTokens.ToList())
            {
                _context.RefreshTokens.Remove(token);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "密码修改成功: {UserId}, 操作人: {OperatorId}, IP: {IpAddress}",
                targetUserId,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "修改密码失败: {UserId}", request.UserId);
            return Result.Fail("修改密码失败");
        }
    }
}
