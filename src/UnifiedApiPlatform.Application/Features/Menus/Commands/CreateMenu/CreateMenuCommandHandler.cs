using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Application.Features.Menus.Commands.CreateMenu;

public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, Result<CreateMenuResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<CreateMenuCommandHandler> _logger;

    public CreateMenuCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<CreateMenuCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<CreateMenuResponse>> Handle(
        CreateMenuCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId;
            var now = _clock.GetCurrentInstant();

            // 创建菜单
            var menu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,  // 租户菜单或系统菜单（null）
                ParentId = request.ParentId,
                Code = request.Code,
                Name = request.Name,
                Type = request.Type,
                PermissionCode = request.PermissionCode,
                Icon = request.Icon,
                Path = request.Path,
                Component = request.Component,
                SortOrder = request.SortOrder,
                IsVisible = request.IsVisible,
                IsActive = true,
                IsSystemMenu = false,  // 用户创建的菜单不是系统菜单
                CreatedAt = now,
                CreatedBy = _currentUser.UserId
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "菜单创建成功: {MenuId}, 名称: {Name}, 操作人: {OperatorId}, IP: {IpAddress}",
                menu.Id,
                menu.Name,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok(new CreateMenuResponse
            {
                Id = menu.Id,
                Code = menu.Code,
                Name = menu.Name
            });
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == "23505") // unique_violation
            {
                string errorMessage = "创建菜单失败";

                if (pgEx.ConstraintName?.Contains("code") == true)
                {
                    errorMessage = "菜单编码已存在";
                }

                _logger.LogWarning("菜单创建失败（唯一性约束）: {Code}, 约束: {Constraint}",
                    request.Code, pgEx.ConstraintName);

                return Result.Fail<CreateMenuResponse>(errorMessage);
            }

            _logger.LogError(ex, "菜单创建失败（数据库错误）: {Code}", request.Code);
            return Result.Fail<CreateMenuResponse>("创建菜单失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "菜单创建失败: {Code}", request.Code);
            return Result.Fail<CreateMenuResponse>("创建菜单失败");
        }
    }
}
