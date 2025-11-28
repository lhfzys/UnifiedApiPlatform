namespace UnifiedApiPlatform.Infrastructure.Options;

public class FileStorageSettings
{
    public const string SectionName = "FileStorage";

    public StorageProvider Provider { get; set; } = StorageProvider.Local;
    public string LocalPath { get; set; } = "/app/uploads";
    public string BaseUrl { get; set; } = null!;
    public int MaxFileSizeInMB { get; set; } = 10;
    public int MaxLargeFileSizeInMB { get; set; } = 50;
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
    public int TempFileExpirationHours { get; set; } = 24;

    public MinioOptions Minio { get; set; } = new();
}

public enum StorageProvider
{
    Local = 0,
    MinIO = 1,
    Azure = 2,
    AWS = 3,
    Aliyun = 4
}

public class MinioOptions
{
    public string Endpoint { get; set; } = null!;
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public bool UseSSL { get; set; } = false;
    public string BucketName { get; set; } = null!;
}
