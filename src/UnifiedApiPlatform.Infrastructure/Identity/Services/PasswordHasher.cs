using System.Text.RegularExpressions;
using Konscious.Security.Cryptography;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Infrastructure.Identity.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 4;
    private const int MemorySize = 65536; // 64 MB
    private const int Parallelism = 1;

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        // 生成随机盐值
        var salt = GenerateSalt();

        // 使用 Argon2id 生成哈希
        var hash = HashPassword(password, salt);

        // 组合盐值和哈希值
        var hashBytes = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(passwordHash))
            return false;

        try
        {
            var hashBytes = Convert.FromBase64String(passwordHash);

            if (hashBytes.Length != SaltSize + HashSize)
                return false;

            // 提取盐值
            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            // 提取存储的哈希
            var storedHash = new byte[HashSize];
            Buffer.BlockCopy(hashBytes, SaltSize, storedHash, 0, HashSize);

            // 使用相同的盐值计算新哈希
            var computedHash = HashPassword(password, salt);

            // 比较哈希值（防止时序攻击）
            return CryptographicEquals(storedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    public PasswordStrength CheckStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return PasswordStrength.Weak;

        var score = 0;

        // 长度检查
        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;
        if (password.Length >= 16) score++;

        // 字符类型检查
        if (Regex.IsMatch(password, @"[a-z]")) score++; // 小写字母
        if (Regex.IsMatch(password, @"[A-Z]")) score++; // 大写字母
        if (Regex.IsMatch(password, @"[0-9]")) score++; // 数字
        if (Regex.IsMatch(password, @"[^a-zA-Z0-9]")) score++; // 特殊字符

        return score switch
        {
            >= 6 => PasswordStrength.VeryStrong,
            >= 4 => PasswordStrength.Strong,
            >= 2 => PasswordStrength.Medium,
            _ => PasswordStrength.Weak
        };
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[SaltSize];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(System.Text.Encoding.UTF8.GetBytes(password))
        {
            Salt = salt, DegreeOfParallelism = Parallelism, MemorySize = MemorySize, Iterations = Iterations
        };

        return argon2.GetBytes(HashSize);
    }

    private static bool CryptographicEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        var result = 0;
        for (var i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }
}
