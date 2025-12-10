using NodaTime;
using UAParser;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Domain.Entities;

namespace UnifiedApiPlatform.Infrastructure.Services;

/// <summary>
/// 审计日志服务实现
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IClock _clock;
    private readonly IIpLocationService _ipLocationService;

    public AuditLogService(IApplicationDbContext context, ICurrentUserService currentUser,
        IClock clock, IIpLocationService ipLocationService)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
        _ipLocationService = ipLocationService;
    }

    public async Task LogOperationAsync(
        string action,
        string httpMethod,
        string requestPath,
        string? requestBody,
        int statusCode,
        string? responseBody,
        long duration,
        string ipAddress,
        string? userAgent,
        string? entityType = null,
        string? entityId = null,
        string? exception = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _currentUser.IsAuthenticated && !string.IsNullOrEmpty(_currentUser.UserId)
                ? Guid.Parse(_currentUser.UserId)
                : (Guid?)null;
            var userName = _currentUser.IsAuthenticated && !string.IsNullOrEmpty(_currentUser.UserName)
                ? _currentUser.UserName
                : null;
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                TenantId = _currentUser.TenantId,
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                HttpMethod = httpMethod,
                RequestPath = requestPath,
                RequestBody = TruncateString(requestBody, 10000), // 限制长度
                StatusCode = statusCode,
                ResponseBody = statusCode >= 400 ? TruncateString(responseBody, 5000) : null, // 只记录错误响应
                Duration = duration,
                IpAddress = ipAddress,
                UserAgent = TruncateString(userAgent, 500),
                Exception = TruncateString(exception, 5000),
                CreatedAt = _clock.GetCurrentInstant(),
                IsSuccess = statusCode >= 200 && statusCode < 400
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // 日志记录失败不应该影响主流程，忽略异常
        }
    }

    public async Task LogLoginAsync(
        string userName,
        string loginType,
        bool isSuccess,
        string ipAddress,
        string? userAgent,
        string? failureReason = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 解析 User-Agent
            var (browser, os, deviceType) = ParseUserAgent(userAgent);

            var location = _ipLocationService.GetFormattedLocation(ipAddress);

            var loginLog = new LoginLog
            {
                Id = Guid.NewGuid(),
                TenantId = _currentUser.TenantId,
                UserId = userId,
                UserName = userName,
                LoginType = loginType,
                IsSuccess = isSuccess,
                FailureReason = failureReason,
                IpAddress = ipAddress,
                UserAgent = TruncateString(userAgent, 500),
                Browser = browser,
                OperatingSystem = os,
                DeviceType = deviceType,
                Location = location,
                CreatedAt = _clock.GetCurrentInstant()
            };

            _context.LoginLogs.Add(loginLog);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // 日志记录失败不应该影响主流程，忽略异常
        }
    }

    /// <summary>
    /// 解析 User-Agent
    /// </summary>
    private static (string? Browser, string? Os, string? DeviceType) ParseUserAgent(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            return (null, null, null);
        }

        try
        {
            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(userAgent);

            var browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}";
            var os = $"{clientInfo.OS.Family} {clientInfo.OS.Major}";
            var deviceType = clientInfo.Device.IsSpider ? "Bot" :
                clientInfo.Device.Family.Contains("Mobile") ? "Mobile" :
                clientInfo.Device.Family.Contains("Tablet") ? "Tablet" : "Desktop";

            return (browser, os, deviceType);
        }
        catch
        {
            return (null, null, null);
        }
    }

    /// <summary>
    /// 截断字符串
    /// </summary>
    private static string? TruncateString(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "...";
    }
}
