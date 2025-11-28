namespace UnifiedApiPlatform.Infrastructure.Options;

public class EmailSettings
{
    public const string SectionName = "Email";

    public string FromEmail { get; set; } = null!;
    public string FromName { get; set; } = null!;
    public string SmtpServer { get; set; } = null!;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool EnableSsl { get; set; } = true;
}
