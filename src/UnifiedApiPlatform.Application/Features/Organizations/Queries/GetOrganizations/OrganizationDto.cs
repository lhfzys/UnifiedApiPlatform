namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizations;

/// <summary>
/// 组织 DTO
/// </summary>
public class OrganizationDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int UserCount { get; set; }  // 用户数量
    public int ChildCount { get; set; }  // 子组织数量
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 组织树节点 DTO
/// </summary>
public class OrganizationTreeNodeDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? FullName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int UserCount { get; set; }
    public List<OrganizationTreeNodeDto> Children { get; set; } = new();
}
