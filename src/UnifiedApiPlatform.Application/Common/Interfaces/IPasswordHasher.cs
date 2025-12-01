namespace UnifiedApiPlatform.Application.Common.Interfaces;

public interface IPasswordHasher
{
    /// <summary>
    /// 哈希密码
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// 验证密码
    /// </summary>
    bool Verify(string password, string passwordHash);

    /// <summary>
    /// 检查密码强度
    /// </summary>
    PasswordStrength CheckStrength(string password);
}

public enum PasswordStrength
{
    Weak = 0,
    Medium = 1,
    Strong = 2,
    VeryStrong = 3
}
