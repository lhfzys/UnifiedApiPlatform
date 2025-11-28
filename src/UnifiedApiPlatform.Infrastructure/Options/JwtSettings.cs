namespace UnifiedApiPlatform.Infrastructure.Options;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
    public int MaxRefreshTokensPerUser { get; set; } = 5;
}
