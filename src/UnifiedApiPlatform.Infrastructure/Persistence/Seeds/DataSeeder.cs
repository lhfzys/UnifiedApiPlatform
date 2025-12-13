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
            await SeedOrganizationsAsync();
            await SeedUsersAsync();
            await SeedRolePermissionsAsync();
            await SeedMenusAsync();


            _logger.LogInformation("种子数据初始化完成！");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "种子数据初始化失败");
            throw;
        }
    }

    /// <summary>
    /// 创建默认租户
    /// </summary>
    private async Task SeedTenantsAsync()
    {
        if (await _context.Tenants.AnyAsync())
        {
            _logger.LogInformation("租户已存在，跳过种子数据");
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

        _logger.LogInformation("已创建默认租户：{TenantName}", tenant.Name);
    }

    /// <summary>
    /// 同步权限定义到数据库
    /// </summary>
    private async Task SeedPermissionsAsync()
    {
        _logger.LogInformation("开始同步权限定义...");

        var definitions = PermissionDefinitions.All;
        _logger.LogInformation("共 {Count} 个权限定义", definitions.Count);

        var existingPermissions = await _context.Permissions
            .Where(p => p.TenantId == "default")
            .ToListAsync();

        _logger.LogInformation("数据库中已有 {Count} 个权限", existingPermissions.Count);

        var now = _clock.GetCurrentInstant();
        var addedCount = 0;
        var updatedCount = 0;

        foreach (var definition in definitions)
        {
            var existing = existingPermissions.FirstOrDefault(p => p.Code == definition.Code);

            if (existing == null)
            {
                var permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    TenantId = "default",
                    Code = definition.Code,
                    Name = definition.Name,
                    Category = definition.Category,
                    Description = definition.Description,
                    IsSystemPermission = true,
                    SortOrder = definition.SortOrder,
                    IsActive = true,
                    CreatedAt = now
                };

                _context.Permissions.Add(permission);
                addedCount++;

                _logger.LogDebug(
                    "添加新权限: {Code} - {Name} (排序: {SortOrder})",
                    definition.Code,
                    definition.Name,
                    definition.SortOrder);
            }
            else
            {
                var changed = false;

                if (existing.Name != definition.Name)
                {
                    existing.Name = definition.Name;
                    changed = true;
                }

                if (existing.Category != definition.Category)
                {
                    existing.Category = definition.Category;
                    changed = true;
                }

                if (existing.Description != definition.Description)
                {
                    existing.Description = definition.Description;
                    changed = true;
                }

                if (existing.SortOrder != definition.SortOrder)
                {
                    existing.SortOrder = definition.SortOrder;
                    changed = true;
                }

                if (existing.IsSystemPermission != definition.IsSystem)
                {
                    existing.IsSystemPermission = definition.IsSystem;
                    changed = true;
                }

                if (changed)
                {
                    existing.UpdatedAt = now;
                    updatedCount++;
                    _logger.LogDebug("更新权限: {Code}", definition.Code);
                }
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "权限同步完成: 新增 {Added} 个, 更新 {Updated} 个",
            addedCount, updatedCount);
    }

    /// <summary>
    /// 创建默认角色
    /// </summary>
    private async Task SeedRolesAsync()
    {
        if (await _context.Roles.AnyAsync())
        {
            _logger.LogInformation("角色已存在，跳过种子数据");
            return;
        }

        var now = _clock.GetCurrentInstant();

        var roles = new[]
        {
            new Role
            {
                Id = Guid.NewGuid(),
                TenantId = "default",
                Code = "SuperAdmin",
                Name = "SuperAdmin",
                DisplayName = "超级管理员",
                Description = "拥有系统所有权限",
                IsSystemRole = true,
                CreatedAt = now
            },
            new Role
            {
                Id = Guid.NewGuid(),
                TenantId = "default",
                Code = "Admin",
                Name = "Admin",
                DisplayName = "管理员",
                Description = "拥有大部分管理权限",
                IsSystemRole = true,
                CreatedAt = now
            },
            new Role
            {
                Id = Guid.NewGuid(),
                TenantId = "default",
                Code = "User",
                Name = "User",
                DisplayName = "普通用户",
                Description = "基本用户权限",
                IsSystemRole = false,
                CreatedAt = now
            }
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();

        _logger.LogInformation("创建 {Count} 个默认角色", roles.Length);
    }

    /// <summary>
    /// 创建默认用户
    /// </summary>
    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("用户已存在，跳过种子数据");
            return;
        }

        var now = _clock.GetCurrentInstant();
        var superAdminRole = await _context.Roles.FirstAsync(r => r.Name == "SuperAdmin");

        var admin = new User
        {
            Id = Guid.NewGuid(),
            TenantId = "default",
            UserName = "admin",
            Email = "admin@example.com",
            PasswordHash = _passwordHasher.Hash("Admin@123"),
            Status = UserStatus.Active,
            IsActive = true,
            IsSystemUser = true,
            CreatedAt = now
        };

        await _context.Users.AddAsync(admin);
        await _context.SaveChangesAsync();

        var userRole = new UserRole
        {
            UserId = admin.Id,
            RoleId = superAdminRole.Id
        };

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();

        _logger.LogInformation("创建默认用户: {UserName}", admin.UserName);
    }

    /// <summary>
    /// 创建默认菜单
    /// </summary>
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
            // ==================== 一级菜单 ====================
            new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = "default",
                Code = "system",
                Name = "系统管理",
                Icon = "settings",
                Path = "/system",
                Type = MenuType.Menu,
                SortOrder = 1000,
                IsVisible = true,
                IsActive = true,
                CreatedAt = now
            },
            new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = "default",
                Code = "users",
                Name = "用户管理",
                Icon = "user",
                Path = "/system/users",
                Type = MenuType.Menu,
                ParentId = null, // 暂时为 null，稍后关联
                SortOrder = 1001,
                IsVisible = true,
                IsActive = true,
                PermissionCode = PermissionCodes.UsersView,
                CreatedAt = now
            },
            new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = "default",
                Code = "roles",
                Name = "角色管理",
                Icon = "team",
                Path = "/system/roles",
                Type = MenuType.Menu,
                SortOrder = 1002,
                IsVisible = true,
                IsActive = true,
                PermissionCode = PermissionCodes.RolesView,
                CreatedAt = now
            }
        };

        // 设置父级关系
        var systemMenu = menus.First(m => m.Code == "system");
        var usersMenu = menus.First(m => m.Code == "users");
        var rolesMenu = menus.First(m => m.Code == "roles");

        usersMenu.ParentId = systemMenu.Id;
        rolesMenu.ParentId = systemMenu.Id;

        await _context.Menus.AddRangeAsync(menus);
        await _context.SaveChangesAsync();

        _logger.LogInformation("创建 {Count} 个默认菜单", menus.Count);

        // 给 SuperAdmin 分配所有菜单
        var superAdminRole = await _context.Roles.FirstAsync(r => r.Name == "SuperAdmin");

        foreach (var menu in menus)
        {
            var roleMenu = new RoleMenu
            {
                RoleId = superAdminRole.Id,
                MenuId = menu.Id
            };
            _context.RoleMenus.Add(roleMenu);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("为 SuperAdmin 角色分配 {Count} 个菜单", menus.Count);
    }

    /// <summary>
    /// 分配权限给角色
    /// </summary>
    private async Task SeedRolePermissionsAsync()
    {
        var superAdminRole = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstAsync(r => r.Name == "SuperAdmin");

        if (superAdminRole.RolePermissions.Any())
        {
            _logger.LogInformation("SuperAdmin 角色已有权限，跳过分配");
            return;
        }

        var allPermissions = await _context.Permissions
            .Where(p => p.TenantId == "default")
            .ToListAsync();

        foreach (var permission in allPermissions)
        {
            var rolePermission = new RolePermission
            {
                RoleId = superAdminRole.Id,
                PermissionId = permission.Id
            };
            _context.RolePermissions.Add(rolePermission);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("为 SuperAdmin 角色分配 {Count} 个权限", allPermissions.Count);
    }

    /// <summary>
/// 创建默认组织
/// </summary>
private async Task SeedOrganizationsAsync()
{
    if (await _context.Organizations.AnyAsync())
    {
        _logger.LogInformation("组织已存在，跳过种子数据");
        return;
    }

    var now = _clock.GetCurrentInstant();

    // 1. 创建公司总部
    var company = new Organization
    {
        Id = Guid.NewGuid(),
        TenantId = "default",
        Code = "HQ",
        Name = "公司总部",
        FullName = "XX科技有限公司",
        Type = OrganizationType.Company,
        Description = "公司总部",
        Level = 1,
        Path = string.Empty, // 临时为空，后面会更新
        SortOrder = 1,
        IsActive = true,
        CreatedAt = now
    };
    company.UpdatePath(string.Empty);

    await _context.Organizations.AddAsync(company);

    // 2. 创建一级部门
    var techDept = new Organization
    {
        Id = Guid.NewGuid(),
        TenantId = "default",
        ParentId = company.Id,
        Code = "TECH",
        Name = "技术部",
        FullName = "技术研发部",
        Type = OrganizationType.Department,
        Description = "负责产品研发",
        Level = 2,
        Path = string.Empty,
        SortOrder = 1,
        IsActive = true,
        CreatedAt = now
    };
    techDept.UpdatePath(company.Path);

    var marketDept = new Organization
    {
        Id = Guid.NewGuid(),
        TenantId = "default",
        ParentId = company.Id,
        Code = "MARKET",
        Name = "市场部",
        FullName = "市场营销部",
        Type = OrganizationType.Department,
        Description = "负责市场推广和销售",
        Level = 2,
        Path = string.Empty,
        SortOrder = 2,
        IsActive = true,
        CreatedAt = now
    };
    marketDept.UpdatePath(company.Path);

    var adminDept = new Organization
    {
        Id = Guid.NewGuid(),
        TenantId = "default",
        ParentId = company.Id,
        Code = "ADMIN",
        Name = "行政部",
        FullName = "行政人事部",
        Type = OrganizationType.Department,
        Description = "负责行政和人事管理",
        Level = 2,
        Path = string.Empty,
        SortOrder = 3,
        IsActive = true,
        CreatedAt = now
    };
    adminDept.UpdatePath(company.Path);

    await _context.Organizations.AddRangeAsync(techDept, marketDept, adminDept);

    // 3. 创建二级团队（技术部下的团队）
    var devTeam = new Organization
    {
        Id = Guid.NewGuid(),
        TenantId = "default",
        ParentId = techDept.Id,
        Code = "DEV",
        Name = "开发组",
        FullName = "软件开发组",
        Type = OrganizationType.Team,
        Description = "负责软件开发",
        Level = 3,
        Path = string.Empty,
        SortOrder = 1,
        IsActive = true,
        CreatedAt = now
    };
    devTeam.UpdatePath(techDept.Path);

    var qaTeam = new Organization
    {
        Id = Guid.NewGuid(),
        TenantId = "default",
        ParentId = techDept.Id,
        Code = "QA",
        Name = "测试组",
        FullName = "质量测试组",
        Type = OrganizationType.Team,
        Description = "负责质量测试",
        Level = 3,
        Path = string.Empty,
        SortOrder = 2,
        IsActive = true,
        CreatedAt = now
    };
    qaTeam.UpdatePath(techDept.Path);

    // 市场部下的团队
    var salesTeam = new Organization
    {
        Id = Guid.NewGuid(),
        TenantId = "default",
        ParentId = marketDept.Id,
        Code = "SALES",
        Name = "销售组",
        FullName = "销售团队",
        Type = OrganizationType.Team,
        Description = "负责产品销售",
        Level = 3,
        Path = string.Empty,
        SortOrder = 1,
        IsActive = true,
        CreatedAt = now
    };
    salesTeam.UpdatePath(marketDept.Path);

    var promotionTeam = new Organization
    {
        Id = Guid.NewGuid(),
        TenantId = "default",
        ParentId = marketDept.Id,
        Code = "PROMOTION",
        Name = "推广组",
        FullName = "市场推广组",
        Type = OrganizationType.Team,
        Description = "负责市场推广",
        Level = 3,
        Path = string.Empty,
        SortOrder = 2,
        IsActive = true,
        CreatedAt = now
    };
    promotionTeam.UpdatePath(marketDept.Path);

    await _context.Organizations.AddRangeAsync(devTeam, qaTeam, salesTeam, promotionTeam);

    await _context.SaveChangesAsync();

    _logger.LogInformation("创建 8 个默认组织");
}
}
