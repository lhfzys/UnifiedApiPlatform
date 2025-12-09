using UnifiedApiPlatform.Application.Common.Commands;

namespace UnifiedApiPlatform.Application.Features.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationCommand : CommandBase<UpdateOrganizationResponse>
{
    public Guid OrganizationId { get; set; }
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
    public string? FullName { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public Guid? ManagerId { get; set; }
    public int? SortOrder { get; set; }
    public byte[]? RowVersion { get; set; }
}

public class UpdateOrganizationResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
