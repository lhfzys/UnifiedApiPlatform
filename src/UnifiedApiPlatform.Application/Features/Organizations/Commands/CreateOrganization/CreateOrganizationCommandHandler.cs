using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.CreateOrganization;

public class
    CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Result<CreateOrganizationResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<CreateOrganizationCommandHandler> _logger;

    public CreateOrganizationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<CreateOrganizationCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<CreateOrganizationResponse>> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";
            var now = _clock.GetCurrentInstant();

            // 查询父组织（如果有）
            Organization? parent = null;
            if (request.ParentId.HasValue)
            {
                parent = await _context.Organizations
                    .FirstOrDefaultAsync(o => o.Id == request.ParentId.Value && o.TenantId == tenantId,
                        cancellationToken);

                if (parent == null)
                {
                    _logger.LogWarning("父组织不存在: {ParentId}", request.ParentId);
                    return Result.Fail<CreateOrganizationResponse>(ErrorCodes.OrganizationNotFound);
                }
            }

            // 创建组织
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = request.ParentId,
                Code = request.Code,
                Name = request.Name,
                FullName = request.FullName ?? request.Name,
                Type = request.Type,
                Description = request.Description,
                ManagerId = request.ManagerId,
                SortOrder = request.SortOrder,
                IsActive = true,
                CreatedAt = now,
                CreatedBy = _currentUser.UserId,
                Path = string.Empty // 临时为空
            };

            // 更新路径和层级
            organization.UpdatePath(parent?.Path ?? string.Empty);
            organization.CalculateLevel();

            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "组织创建成功: {OrganizationId}, 名称: {Name}, 操作人: {OperatorId}, IP: {IpAddress}",
                organization.Id,
                organization.Name,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok(new CreateOrganizationResponse
            {
                Id = organization.Id, Code = organization.Code, Name = organization.Name
            });
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == "23505") // unique_violation
            {
                string errorMessage = "创建组织失败";

                if (pgEx.ConstraintName?.Contains("code") == true)
                {
                    errorMessage = "组织编码已存在";
                }

                _logger.LogWarning("组织创建失败（唯一性约束）: {Code}, 约束: {Constraint}",
                    request.Code, pgEx.ConstraintName);

                return Result.Fail<CreateOrganizationResponse>(errorMessage);
            }

            _logger.LogError(ex, "组织创建失败（数据库错误）: {Code}", request.Code);
            return Result.Fail<CreateOrganizationResponse>("创建组织失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "组织创建失败: {Code}", request.Code);
            return Result.Fail<CreateOrganizationResponse>("创建组织失败");
        }
    }
}
