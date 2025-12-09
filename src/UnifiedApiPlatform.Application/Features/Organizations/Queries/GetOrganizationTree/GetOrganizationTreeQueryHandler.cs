using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizations;

namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationTree;

public class
    GetOrganizationTreeQueryHandler : IRequestHandler<GetOrganizationTreeQuery, Result<List<OrganizationTreeNodeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetOrganizationTreeQueryHandler> _logger;

    public GetOrganizationTreeQueryHandler(
        IApplicationDbContext context,
        ILogger<GetOrganizationTreeQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<OrganizationTreeNodeDto>>> Handle(GetOrganizationTreeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Organizations
                .AsNoTracking()
                .Where(o => o.TenantId == request.CurrentTenantId);

            // 是否包含未激活的
            if (!request.IncludeInactive)
            {
                query = query.Where(o => o.IsActive);
            }

            // 查询所有组织
            var allOrganizations = await query
                .OrderBy(o => o.SortOrder)
                .ThenBy(o => o.Name)
                .Select(o => new OrganizationTreeNodeDto
                {
                    Id = o.Id,
                    ParentId = o.ParentId,
                    Code = o.Code,
                    Name = o.Name,
                    FullName = o.FullName,
                    SortOrder = o.SortOrder,
                    IsActive = o.IsActive,
                    UserCount = o.Users.Count,
                    Children = new List<OrganizationTreeNodeDto>()
                })
                .ToListAsync(cancellationToken);

            // 构建树形结构
            var tree = BuildTree(allOrganizations, null);

            return Result.Ok(tree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取组织树失败");
            return Result.Fail<List<OrganizationTreeNodeDto>>("获取组织树失败");
        }
    }

    /// <summary>
    /// 构建树形结构（递归）
    /// </summary>
    private static List<OrganizationTreeNodeDto> BuildTree(
        List<OrganizationTreeNodeDto> allNodes,
        Guid? parentId)
    {
        return allNodes
            .Where(n => n.ParentId == parentId)
            .Select(n =>
            {
                n.Children = BuildTree(allNodes, n.Id);
                return n;
            })
            .OrderBy(n => n.SortOrder)
            .ThenBy(n => n.Name)
            .ToList();
    }
}
