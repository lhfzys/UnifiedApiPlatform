using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Infrastructure.Persistence;
using UnifiedApiPlatform.Infrastructure.Persistence.Seeds;

namespace UnifiedApiPlatform.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();

        try
        {
            // 应用待处理的迁移
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var migrations = pendingMigrations as string[] ?? pendingMigrations.ToArray();
            if (migrations.Length != 0)
            {
                logger.LogInformation("正在应用 {Count} 个待处理的迁移...", migrations.Length);
                await context.Database.MigrateAsync();
                logger.LogInformation("迁移应用完成");
            }

            // 执行种子数据
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "数据库初始化失败");
            throw;
        }
    }
}
