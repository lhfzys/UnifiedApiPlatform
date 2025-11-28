namespace UnifiedApiPlatform.Infrastructure.Options;

public class DatabaseSettings
{
    public const string SectionName = "Database";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int MinPoolSize { get; set; } = 5;
    public int MaxPoolSize { get; set; } = 20;
    public int ConnectionTimeout { get; set; } = 30;
    public int CommandTimeout { get; set; } = 60;
    public bool EnableSensitiveDataLogging { get; set; } = false;

    public string GetConnectionString()
    {
        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};" +
               $"Minimum Pool Size={MinPoolSize};Maximum Pool Size={MaxPoolSize};" +
               $"Timeout={ConnectionTimeout};Command Timeout={CommandTimeout}";
    }
}
