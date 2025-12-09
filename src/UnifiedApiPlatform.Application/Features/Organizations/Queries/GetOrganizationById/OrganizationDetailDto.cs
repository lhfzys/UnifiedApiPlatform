namespace UnifiedApiPlatform.Application.Features.Organizations.Queries.GetOrganizationById;

public class OrganizationDetailDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int UserCount { get; set; }
    public int ChildCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public string? RowVersion { get; set; }
}
