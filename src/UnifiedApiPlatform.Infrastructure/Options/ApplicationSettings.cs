namespace UnifiedApiPlatform.Infrastructure.Options;

public class ApplicationSettings
{
    public const string SectionName = "Application";

    public string Name { get; set; } = "UnifiedApiPlatform";
    public string Version { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";
    public bool EnableSwagger { get; set; } = true;
    public bool EnableDetailedErrors { get; set; } = true;
    public string? SupportEmail { get; set; }
}
