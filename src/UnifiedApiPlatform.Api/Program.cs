using FastEndpoints.Swagger;
using Serilog;
using UnifiedApiPlatform.Api.Extensions;
using UnifiedApiPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 配置 Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

try
{
    Log.Information("Starting UnifiedApiPlatform API");

    // 添加服务
    builder.Services.AddApiServices(builder.Configuration);

    var app = builder.Build();

    // 数据库初始化（包含种子数据）
    if (app.Environment.IsDevelopment())
    {
        await app.Services.InitializeDatabaseAsync();
    }

    // 配置应用
    app.ConfigureApplication();

    Log.Information("Application configured successfully");

    Log.Information("========== 检查预处理器注册 ==========");
    var serviceDescriptors = builder.Services
        .Where(s => s.ServiceType.Name.Contains("PreProcessor"))
        .ToList();
    foreach (var descriptor in serviceDescriptors)
    {
        Log.Information("已注册的预处理器: {ServiceType} -> {ImplementationType}",
            descriptor.ServiceType,
            descriptor.ImplementationType);
    }
    Log.Information("========== 预处理器检查结束 ==========");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
