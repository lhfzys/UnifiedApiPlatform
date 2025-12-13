using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<CreateTenantResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IClock _clock;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordHasher _passwordHasher;

    public CreateTenantCommandHandler(
        IApplicationDbContext context,
        IClock clock,
        ICurrentUserService currentUser,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _clock = clock;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<CreateTenantResponse>> Handle(
        CreateTenantCommand request,
        CancellationToken cancellationToken)
    {
        // 1. 检查租户标识是否已存在
        var exists = await _context.Tenants
            .AnyAsync(t => t.Identifier == request.Identifier && !t.IsDeleted, cancellationToken);

        if (exists)
        {
            return Result.Fail<CreateTenantResponse>(ErrorCodes.TenantIdentifierAlreadyExists);
        }

        // 2. 创建租户
        var now = _clock.GetCurrentInstant();
        var tenantId = request.Identifier;
        Console.WriteLine($"{tenantId}", "创建租户");
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Identifier = request.Identifier,
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            ActivatedAt = request.IsActive ? now : null,
            ContactName = request.ContactName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            MaxUsers = request.MaxUsers,
            MaxStorageInBytes = request.MaxStorageInGB * 1024L * 1024 * 1024,
            StorageUsedInBytes = 0,
            MaxApiCallsPerDay = request.MaxApiCallsPerDay,
            CreatedAt = now,
            CreatedBy = _currentUser.UserName ?? "System"
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        // 3. 自动初始化租户数据（可选）
        if (request.AutoInitialize)
        {
            await InitializeTenantDataAsync(tenant.Id.ToString(),tenant.Identifier, cancellationToken);
        }

        // 4. 返回结果
        var response = new CreateTenantResponse
        {
            Id = tenant.Id.ToString(),
            Identifier = tenant.Identifier,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt.ToDateTimeUtc()
        };

        return Result.Ok(response);
    }

    /// <summary>
    /// 初始化租户数据（角色、权限、管理员用户）
    /// </summary>
    private async Task InitializeTenantDataAsync(string tenantId,string tenantIdentifier, CancellationToken cancellationToken)
    {
        var now = _clock.GetCurrentInstant();

        var tenantExists = await _context.Tenants
            .AnyAsync(t => t.Id.ToString() == tenantId && !t.IsDeleted, cancellationToken);

        if (!tenantExists)
        {
            throw new InvalidOperationException($"租户 {tenantId} 不存在");
        }
        // 1. 复制权限到新租户（从默认租户复制）
        var defaultPermissions = await _context.Permissions
            .Where(p => p.TenantId == "default" && !p.IsDeleted)
            .ToListAsync(cancellationToken);

        var tenantPermissions = defaultPermissions.Select(p => new Permission
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Code = p.Code,
            Name = p.Name,
            Category = p.Category,
            Description = p.Description,
            SortOrder = p.SortOrder,
            IsSystemPermission = p.IsSystemPermission,
            IsActive = true,
            CreatedAt = now,
            CreatedBy = "System"
        }).ToList();

        _context.Permissions.AddRange(tenantPermissions);
        await _context.SaveChangesAsync(cancellationToken);
        // 2. 创建管理员角色
        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Code = "admin",
            Name = "admin",
            DisplayName = "系统管理员",
            Description = "拥有系统所有权限",
            IsSystemRole = true,
            IsActive = true,
            Sort = 0,
            CreatedAt = now,
            CreatedBy = "System"
        };

        _context.Roles.Add(adminRole);
        await _context.SaveChangesAsync(cancellationToken);

        // 3. 给管理员角色分配所有权限
        var rolePermissions = tenantPermissions.Select(p => new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = adminRole.Id,
            PermissionId = p.Id,
            CreatedAt = now,
            CreatedBy = "System"
        }).ToList();

        _context.RolePermissions.AddRange(rolePermissions);
        await _context.SaveChangesAsync(cancellationToken);
        // 4. 创建管理员用户
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserName = "admin",
            Email = $"admin@{tenantId}.com",
            PasswordHash = _passwordHasher.Hash("Admin@123"),
            IsActive = true,
            CreatedAt = now,
            CreatedBy = "System"
        };

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync(cancellationToken);

        // 5. 给管理员用户分配角色
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = adminUser.Id,
            RoleId = adminRole.Id,
            CreatedAt = now,
            CreatedBy = "System"
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
