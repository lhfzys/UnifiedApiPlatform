using FastEndpoints;
using MediatR;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Application.Features.Tenants.Commands.CreateTenant;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Api.Endpoints.Tenants;

/// <summary>
/// 创建租户端点
/// </summary>
public class CreateTenantEndpoint : Endpoint<CreateTenantRequest>
{
    private readonly IMediator _mediator;

    public CreateTenantEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("tenants");
        Permissions(PermissionCodes.TenantsCreate);
        Summary(s =>
        {
            s.Summary = "创建租户";
            s.Description = "创建新租户，支持自动初始化租户数据（角色、权限、管理员用户）";
        });
    }

    public override async Task HandleAsync(CreateTenantRequest req, CancellationToken ct)
    {
        var command = new CreateTenantCommand
        {
            Identifier = req.Identifier,
            Name = req.Name,
            Description = req.Description,
            ContactName = req.ContactName,
            ContactEmail = req.ContactEmail,
            ContactPhone = req.ContactPhone,
            MaxUsers = req.MaxUsers,
            MaxStorageInGB = req.MaxStorageInGB,
            MaxApiCallsPerDay = req.MaxApiCallsPerDay,
            IsActive = req.IsActive,
            AutoInitialize = req.AutoInitialize,
            TraceId = HttpContext.TraceIdentifier
        };

        var result = await _mediator.Send(command, ct);

        await this.SendResultAsync(result, ct);
    }
}

/// <summary>
/// 创建租户请求
/// </summary>
public class CreateTenantRequest
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
