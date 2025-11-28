namespace UnifiedApiPlatform.Infrastructure.Options;

public class HangfireSettings
{
    public const string SectionName = "Hangfire";

    public int WorkerCount { get; set; } = 10;
    public int JobRetryAttempts { get; set; } = 3;
    public bool EnableDashboard { get; set; } = true;
    public string DashboardPath { get; set; } = "/hangfire";
    public string? DashboardUsername { get; set; }
    public string? DashboardPassword { get; set; }
}
