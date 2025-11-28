namespace UnifiedApiPlatform.Infrastructure.Options;

public class CorsSettings
{
    public const string SectionName = "Cors";

    public string PolicyName { get; set; } = "DefaultCorsPolicy";
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public bool AllowCredentials { get; set; } = true;
}
