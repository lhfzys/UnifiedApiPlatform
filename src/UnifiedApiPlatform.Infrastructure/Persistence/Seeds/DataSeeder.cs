
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Domain.Enums;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Infrastructure.Persistence.Seeds;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IClock _clock;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IClock clock,
        ILogger<DataSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _clock = clock;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("开始执行种子数据初始化...");

            await SeedTenantsAsync();
            await SeedPermissionsAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedMenusAsync();

            _logger.LogInformation("种子数据初始化完成！");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "种子数据初始化失败");
            throw;
        }
    }

    private async Task SeedTenantsAsync()
    {
        if (await _context.Tenants.AnyAsync())
        {
            _logger.LogInformation("租户数据已存在，跳过初始化");
            return;
        }

        var now = _clock.GetCurrentInstant();
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Identifier = "default",
            Name = "默认租户",
            Description = "系统默认租户",
            IsActive = true,
            ActivatedAt = now,
            MaxUsers = 100,
            MaxStorageInBytes = 10L * 1024 * 1024 * 1024, // 10GB
            ContactEmail = "admin@example.com",
            CreatedAt = now
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("已创建默认租户：{Identifier}", tenant.Identifier);
    }

    private async Task SeedPermissionsAsync()
    {
        if (await _context.Permissions.AnyAsync())
        {
            _logger.LogInformation("权限数据已存在，跳过初始化");
            return;
        }

        var permissions = new List<Permission>
        {
            // 用户管理
            new() { Code = PermissionCodes.UsersView, Name = "查看用户", Category = "用户管理", IsSystemPermission = true },
            new() { Code = PermissionCodes.UsersCreate, Name = "创建用户", Category = "用户管理", IsSystemPermission = true },
            new() { Code = PermissionCodes.UsersUpdate, Name = "更新用户", Category = "用户管理", IsSystemPermission = true },
            new() { Code = PermissionCodes.UsersDelete, Name = "删除用户", Category = "用户管理", IsSystemPermission = true },

            // 角色管理
            new() { Code = PermissionCodes.RolesView, Name = "查看角色", Category = "角色管理", IsSystemPermission = true },
            new() { Code = PermissionCodes.RolesCreate, Name = "创建角色", Category = "角色管理", IsSystemPermission = true },
            new() { Code = PermissionCodes.RolesUpdate, Name = "更新角色", Category = "角色管理", IsSystemPermission = true },
            new() { Code = PermissionCodes.RolesDelete, Name = "删除角色", Category = "角色管理", IsSystemPermission = true },

            // 权限管理
            new() { Code = PermissionCodes.PermissionsView, Name = "查看权限", Category = "权限管理", IsSystemPermission = true },

            // 菜单管理
            new() { Code = PermissionCodes.MenusView, Name = "查看菜单", Category = "菜单管理", IsSystemPermission = true },
            new() { Code = PermissionCodes.MenusCreate, Name = "创建菜单", Category = "菜单管理", IsSystemPermission = true },
            new() { Code = PermissionCodes.MenusUpdate, Name = "更新菜单", Category = "菜单管理", IsSystemPermission = true },
            new() { Code = PermissionCodes.MenusDelete, Name = "删除菜单", Category = "菜单管理", IsSystemPermission = true },
        };

        _context.Permissions.AddRange(permissions);
        await _context.SaveChangesAsync();

        _logger.LogInformation("已创建 {Count} 个系统权限", permissions.Count);
    }

    private async Task SeedRolesAsync()
    {
        var tenant = await _context.Tenants.FirstAsync(t => t.Identifier == "default");

        if (await _context.Roles.AnyAsync(r => r.TenantId == tenant.Identifier))
        {
            _logger.LogInformation("角色数据已存在，跳过初始化");
            return;
        }

        var now = _clock.GetCurrentInstant();

        // 创建超级管理员角色
        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Identifier,
            Name = "SuperAdmin",
            DisplayName = "超级管理员",
            Description = "拥有所有权限的超级管理员",
            IsSystemRole = true,
            Sort = 1,
            CreatedAt = now
        };

        _context.Roles.Add(adminRole);
        await _context.SaveChangesAsync();

        // 为超级管理员分配所有权限
        var allPermissions = await _context.Permissions.ToListAsync();
        var rolePermissions = allPermissions.Select(p => new RolePermission
        {
            RoleId = adminRole.Id,
            PermissionCode = p.Code,
            CreatedAt = now
        }).ToList();

        _context.RolePermissions.AddRange(rolePermissions);
        await _context.SaveChangesAsync();

        _logger.LogInformation("已创建超级管理员角色并分配 {Count} 个权限", allPermissions.Count);
    }

    private async Task SeedUsersAsync()
    {
        var tenant = await _context.Tenants.FirstAsync(t => t.Identifier == "default");

        if (await _context.Users.AnyAsync(u => u.TenantId == tenant.Identifier))
        {
            _logger.LogInformation("用户数据已存在，跳过初始化");
            return;
        }

        var now = _clock.GetCurrentInstant();
        var adminRole = await _context.Roles.FirstAsync(r => r.Name == "SuperAdmin" && r.TenantId == tenant.Identifier);

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Identifier,
            UserName = "admin",
            Email = "admin@example.com",
            PasswordHash = _passwordHasher.Hash("Admin@123"),
            Status = UserStatus.Active,
            IsActive = true,
            CreatedAt = now
        };

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        // 分配角色
        var userRole = new UserRole
        {
            UserId = adminUser.Id,
            RoleId = adminRole.Id,
            CreatedAt = now
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        _logger.LogInformation("已创建管理员用户：{Email}", adminUser.Email);
    }

    private async Task SeedMenusAsync()
    {
        if (await _context.Menus.AnyAsync())
        {
            _logger.LogInformation("菜单数据已存在，跳过初始化");
            return;
        }

        var now = _clock.GetCurrentInstant();
        var menus = new List<Menu>
        {
            // 系统管理
            new()
            {
                Id = Guid.NewGuid(),
                Name = "system",
                Title = "系统管理",
                Type = MenuType.Directory,
                Icon = "Settings",
                Sort = 100,
                IsVisible = true,
                IsSystemMenu = true,
                CreatedAt = now
            }
        };

        var systemMenu = menus.First();

        // 系统管理子菜单
        menus.AddRange(new[]
        {
            new Menu
            {
                Id = Guid.NewGuid(),
                ParentId = systemMenu.Id,
                Name = "users",
                Title = "用户管理",
                Type = MenuType.Menu,
                Path = "/system/users",
                Component = "system/users/index",
                PermissionCode = PermissionCodes.UsersView,
                Icon = "User",
                Sort = 1,
                IsVisible = true,
                IsSystemMenu = true,
                CreatedAt = now
            },
            new Menu
            {
                Id = Guid.NewGuid(),
                ParentId = systemMenu.Id,
                Name = "roles",
                Title = "角色管理",
                Type = MenuType.Menu,
                Path = "/system/roles",
                Component = "system/roles/index",
                PermissionCode = PermissionCodes.RolesView,
                Icon = "UserGroup",
                Sort = 2,
                IsVisible = true,
                IsSystemMenu = true,
                CreatedAt = now
            },
            new Menu
            {
                Id = Guid.NewGuid(),
                ParentId = systemMenu.Id,
                Name = "menus",
                Title = "菜单管理",
                Type = MenuType.Menu,
                Path = "/system/menus",
                Component = "system/menus/index",
                PermissionCode = PermissionCodes.MenusView,
                Icon = "Menu",
                Sort = 3,
                IsVisible = true,
                IsSystemMenu = true,
                CreatedAt = now
            }
        });

        _context.Menus.AddRange(menus);
        await _context.SaveChangesAsync();

        _logger.LogInformation("已创建 {Count} 个系统菜单", menus.Count);
    }
}
