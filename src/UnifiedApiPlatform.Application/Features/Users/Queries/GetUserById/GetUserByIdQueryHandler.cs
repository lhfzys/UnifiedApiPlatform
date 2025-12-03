using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IApplicationDbContext context,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<UserDetailDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogWarning("用户ID: {request.UserId}", request.UserId);
            _logger.LogInformation("开始获取用户详情: {request.TenantId}", request.CurrentTenantId);
            // 查询用户（包含关联数据）
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == request.UserId && u.TenantId == request.CurrentTenantId)
                .Include(u => u.Organization)
                .Include(u => u.Manager)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("用户不存在: {UserId}", request.UserId);
                return Result.Fail<UserDetailDto>(ErrorCodes.UserNotFound);
            }

            // 获取所有权限（去重）
            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            // 映射到 DTO
            var dto = new UserDetailDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                Status = user.Status,
                IsActive = user.IsActive,
                LockedUntil = user.LockedUntil,
                LoginFailureCount = user.LoginFailureCount,
                LastLoginAt = user.LastLoginAt,
                LastLoginIp = user.LastLoginIp,
                PasswordChangedAt = user.PasswordChangedAt,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                UpdatedAt = user.UpdatedAt,
                UpdatedBy = user.UpdatedBy,
                OrganizationId = user.OrganizationId,
                OrganizationName = user.Organization?.Name,
                ManagerId = user.ManagerId,
                ManagerName = user.Manager?.UserName,
                Roles = user.UserRoles.Select(ur => new RoleDto
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    DisplayName = ur.Role.DisplayName,
                    Description = ur.Role.Description
                }).ToList(),
                Permissions = permissions
            };

            _logger.LogInformation("成功获取用户详情: {UserId}", request.UserId);

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户详情失败: {UserId}", request.UserId);
            return Result.Fail<UserDetailDto>("获取用户详情失败");
        }
    }
}
