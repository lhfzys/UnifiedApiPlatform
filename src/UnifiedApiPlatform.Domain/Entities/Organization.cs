using UnifiedApiPlatform.Domain.Common;

namespace UnifiedApiPlatform.Domain.Entities;

/// <summary>
/// 组织架构实体
/// </summary>
public class Organization : MultiTenantEntity
{
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!; // 唯一编码
    public string Type { get; set; } = null!; // Company/Department/Team
    public Guid? ManagerId { get; set; }
    public int Level { get; set; } // 层级
    public string Path { get; set; } = null!; // 路径，如 /1/2/5
    public int Sort { get; set; }

    // 导航属性
    public Organization? Parent { get; set; }
    public ICollection<Organization> Children { get; set; } = new List<Organization>();
    public User? Manager { get; set; }
    public ICollection<User> Members { get; set; } = new List<User>();
}
