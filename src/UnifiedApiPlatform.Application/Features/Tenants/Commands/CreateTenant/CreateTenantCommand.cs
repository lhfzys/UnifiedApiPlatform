using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Tenants.Commands.CreateTenant;

/// <summary>
/// 创建租户命令
/// </summary>
public class CreateTenantCommand : CommandBase<CreateTenantResponse>
{
    /// <summary>
    /// 租户标识（唯一）
    /// </summary>
    public string Identifier { get; set; } = null!;

    /// <summary>
    /// 租户名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 联系人姓名
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系人邮箱
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 联系人电话
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 最大用户数（默认 100）
    /// </summary>
    public int MaxUsers { get; set; } = 100;

    /// <summary>
    /// 最大存储空间（GB，默认 10GB）
    /// </summary>
    public int MaxStorageInGB { get; set; } = 10;

    /// <summary>
    /// 每日最大 API 调用次数（默认 100000）
    /// </summary>
    public int MaxApiCallsPerDay { get; set; } = 100000;

    /// <summary>
    /// 是否立即激活（默认 true）
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 是否自动初始化租户数据（默认 true）
    /// </summary>
    public bool AutoInitialize { get; set; } = true;
}

/// <summary>
/// 创建租户响应
/// </summary>
public class CreateTenantResponse
{
    public string Id { get; set; } = null!;
    public string Identifier { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
