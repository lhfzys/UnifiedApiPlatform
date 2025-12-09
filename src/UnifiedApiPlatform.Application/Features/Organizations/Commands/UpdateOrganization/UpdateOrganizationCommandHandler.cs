using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Shared.Constants;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, Result<UpdateOrganizationResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly ILogger<UpdateOrganizationCommandHandler> _logger;

    public UpdateOrganizationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IClock clock,
        ILogger<UpdateOrganizationCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<UpdateOrganizationResponse>> Handle(
        UpdateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = _currentUser.TenantId ?? "default";

            // 查询组织
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == request.OrganizationId && o.TenantId == tenantId,
                    cancellationToken);

            if (organization == null)
            {
                _logger.LogWarning("组织不存在: {OrganizationId}", request.OrganizationId);
                return Result.Fail<UpdateOrganizationResponse>(ErrorCodes.OrganizationNotFound);
            }

            // 乐观并发控制
            if (request.RowVersion != null && !organization.RowVersion!.SequenceEqual(request.RowVersion))
            {
                _logger.LogWarning("组织数据已被修改: {OrganizationId}", request.OrganizationId);
                return Result.Fail<UpdateOrganizationResponse>("组织数据已被其他人修改，请刷新后重试");
            }

            // 记录原始 ParentId
            var oldParentId = organization.ParentId;

            // 更新字段（只更新提供的字段）
            if (!string.IsNullOrEmpty(request.Name))
            {
                organization.Name = request.Name;
            }

            if (request.FullName != null)
            {
                organization.FullName = string.IsNullOrEmpty(request.FullName) ? organization.Name : request.FullName;
            }

            if (!string.IsNullOrEmpty(request.Type))
            {
                organization.Type = request.Type;
            }

            if (request.Description != null)
            {
                organization.Description = string.IsNullOrEmpty(request.Description) ? null : request.Description;
            }

            if (request.ManagerId.HasValue)
            {
                organization.ManagerId = request.ManagerId.Value;
            }

            if (request.SortOrder.HasValue)
            {
                organization.SortOrder = request.SortOrder.Value;
            }

            // 如果父组织发生变化，更新路径和层级
            if (request.ParentId != oldParentId)
            {
                organization.ParentId = request.ParentId;

                // 获取新父组织
                string newParentPath = string.Empty;
                if (request.ParentId.HasValue)
                {
                    var newParent = await _context.Organizations
                        .FirstOrDefaultAsync(o => o.Id == request.ParentId.Value, cancellationToken);

                    if (newParent != null)
                    {
                        newParentPath = newParent.Path;
                    }
                }

                // 更新当前组织的路径和层级
                var oldPath = organization.Path;
                organization.UpdatePath(newParentPath);
                organization.CalculateLevel();

                // 更新所有子组织的路径和层级
                await UpdateChildrenPathAsync(organization.Id, oldPath, organization.Path, cancellationToken);
            }

            // 更新审计字段
            organization.UpdatedAt = _clock.GetCurrentInstant();
            organization.UpdatedBy = _currentUser.UserId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "组织更新成功: {OrganizationId}, 名称: {Name}, 操作人: {OperatorId}, IP: {IpAddress}",
                organization.Id,
                organization.Name,
                _currentUser.UserId,
                request.IpAddress
            );

            return Result.Ok(new UpdateOrganizationResponse
            {
                Id = organization.Id,
                Code = organization.Code,
                Name = organization.Name
            });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "组织更新冲突: {OrganizationId}", request.OrganizationId);
            return Result.Fail<UpdateOrganizationResponse>("组织数据已被其他人修改，请刷新后重试");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "组织更新失败: {OrganizationId}", request.OrganizationId);
            return Result.Fail<UpdateOrganizationResponse>("更新组织失败");
        }
    }

    /// <summary>
    /// 递归更新所有子组织的路径
    /// </summary>
    private async Task UpdateChildrenPathAsync(
        Guid organizationId,
        string oldPath,
        string newPath,
        CancellationToken cancellationToken)
    {
        var children = await _context.Organizations
            .Where(o => o.ParentId == organizationId)
            .ToListAsync(cancellationToken);

        foreach (var child in children)
        {
            var oldChildPath = child.Path;
            child.Path = child.Path.Replace(oldPath, newPath);
            child.CalculateLevel();

            // 递归更新子组织的子组织
            await UpdateChildrenPathAsync(child.Id, oldChildPath, child.Path, cancellationToken);
        }
    }
}
