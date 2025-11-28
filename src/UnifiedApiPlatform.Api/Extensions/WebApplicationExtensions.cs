using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;
using UnifiedApiPlatform.Infrastructure.Persistence;

namespace UnifiedApiPlatform.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        // Serilog 请求日志
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            };
        });

        // CORS
        app.UseCors("DefaultCorsPolicy");

        // HTTPS 重定向
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        // FastEndpoints
        app.UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
            config.Versioning.Prefix = "v";
            config.Versioning.DefaultVersion = 1;
            config.Versioning.PrependToRoute = true;
        });

        // Swagger (仅在开发环境)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerGen();
        }

        // 数据库初始化（开发环境）
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // 后续会添加：
            // - 自动迁移
            // - 种子数据
        }

        return app;
    }
}
