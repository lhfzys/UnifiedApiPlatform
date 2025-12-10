using System.Reflection;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Infrastructure.Identity.Services;
using UnifiedApiPlatform.Infrastructure.Options;
using UnifiedApiPlatform.Infrastructure.Persistence;
using UnifiedApiPlatform.Infrastructure.Persistence.Interceptors;
using UnifiedApiPlatform.Infrastructure.Persistence.Seeds;
using UnifiedApiPlatform.Infrastructure.Services;

namespace UnifiedApiPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        // 配置 Options
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
        services.Configure<HangfireSettings>(configuration.GetSection(HangfireSettings.SectionName));
        services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.SectionName));
        services.Configure<ApplicationSettings>(configuration.GetSection(ApplicationSettings.SectionName));

        // NodaTime Clock
        services.AddSingleton<IClock>(SystemClock.Instance);
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // HttpContextAccessor 和 CurrentUserService
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // 认证服务
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // 拦截器
        // services.AddScoped<AuditInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<DomainEventInterceptor>();

        // 数据库上下文
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>()!;
            var connectionString = databaseSettings.GetConnectionString();

            options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    npgsqlOptions.UseNodaTime();
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                })
                .EnableSensitiveDataLogging(databaseSettings.EnableSensitiveDataLogging)
                .EnableDetailedErrors(databaseSettings.EnableSensitiveDataLogging);

            // 添加拦截器
            // var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
            var softDeleteInterceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();
            var domainEventInterceptor = serviceProvider.GetRequiredService<DomainEventInterceptor>();

            options.AddInterceptors(softDeleteInterceptor, domainEventInterceptor);
        });

        // 种子数据服务
        services.AddScoped<DataSeeder>();

        // 添加审计日志服务
        services.AddScoped<IAuditLogService, AuditLogService>();

        // ==================== 注册 IP 定位服务 ====================
        // 获取 IP 数据库文件路径
        var ipDbPath = configuration["IpLocation:DatabasePath"] ?? "Data/ip2region.xdb";
        var fullPath = Path.Combine(AppContext.BaseDirectory, ipDbPath);
        // 检查文件是否存在
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                $"IP2Region 数据库文件不存在: {fullPath}。" +
                "请从 https://github.com/lionsoul2014/ip2region 下载 ip2region.xdb 文件。");
        }

        // 注册 IP2Region Searcher（单例）
        services.AddSingleton<ISearcher>(sp =>
        {
            return new Searcher(CachePolicy.Content, fullPath);
        });

        // 注册 IP 定位服务
        services.AddScoped<IIpLocationService, IpLocationService>();

        return services;
    }
}
