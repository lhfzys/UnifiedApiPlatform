namespace UnifiedApiPlatform.Domain.Enums;

/// <summary>
/// 审计操作类型
/// </summary>
public static class AuditAction
{
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";
    public const string Query = "Query";
    public const string Login = "Login";
    public const string Logout = "Logout";
    public const string RefreshToken = "RefreshToken";
    public const string ChangePassword = "ChangePassword";
    public const string Export = "Export";
    public const string Import = "Import";
}
