namespace UnifiedApiPlatform.Infrastructure.Options;

public class RedisSettings
{
    public const string SectionName = "Redis";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6379;
    public string? Password { get; set; }
    public string InstanceName { get; set; } = "UnifiedApiPlatform";
    public int Database { get; set; } = 0;
    public int ConnectTimeout { get; set; } = 5000;
    public int SyncTimeout { get; set; } = 5000;

    public string GetConnectionString()
    {
        var conn = $"{Host}:{Port}";
        if (!string.IsNullOrWhiteSpace(Password))
            conn += $",password={Password}";
        conn += $",defaultDatabase={Database},connectTimeout={ConnectTimeout},syncTimeout={SyncTimeout}";
        return conn;
    }
}
