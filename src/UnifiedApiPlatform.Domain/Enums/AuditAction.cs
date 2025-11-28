namespace UnifiedApiPlatform.Domain.Enums;

public enum AuditAction
{
    Create = 0,
    Update = 1,
    Delete = 2,
    View = 3,
    Login = 4,
    Logout = 5,
    PasswordChange = 6,
    PasswordReset = 7,
    PermissionGrant = 8,
    PermissionRevoke = 9,
    Export = 10,
    Import = 11
}
