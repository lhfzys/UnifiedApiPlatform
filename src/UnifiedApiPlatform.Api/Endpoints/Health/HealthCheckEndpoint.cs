using FastEndpoints;
using UnifiedApiPlatform.Infrastructure.Persistence;

namespace UnifiedApiPlatform.Api.Endpoints.Health;

public class HealthCheckResponse
{
    public string Status { get; set; } = null!;
    public string Service { get; set; } = null!;
    public string Version { get; set; } = null!;
    public string Environment { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Dependencies { get; set; } = new();
}

public class HealthCheckEndpoint : EndpointWithoutRequest<HealthCheckResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public HealthCheckEndpoint(
        ApplicationDbContext dbContext,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _environment = environment;
    }

    public override void Configure()
    {
        Get("health");
        AllowAnonymous();
        Description(d => d
            .WithTags("Health")
            .WithSummary("健康检查端点")
            .WithDescription("检查 API 和依赖服务的健康状态"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new HealthCheckResponse
        {
            Status = "Healthy",
            Service = "UnifiedApiPlatform API",
            Version = _configuration.GetValue<string>("Application:Version") ?? "1.0.0",
            Environment = _environment.EnvironmentName,
            Timestamp = DateTime.UtcNow
        };

        // 检查数据库连接
        try
        {
            await _dbContext.Database.CanConnectAsync(ct);
            response.Dependencies["Database"] = "Connected";
        }
        catch
        {
            response.Dependencies["Database"] = "Disconnected";
            response.Status = "Unhealthy";
        }

        await Send.OkAsync(response, cancellation: ct);
    }
}
