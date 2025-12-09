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
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // 应用迁移
            await context.Database.MigrateAsync();
            logger.LogInformation("数据库迁移完成");

            // 执行种子数据
            await seeder.SeedAsync();

            logger.LogInformation("数据库初始化完成");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "数据库初始化失败");
            throw;
        }
    }
}
