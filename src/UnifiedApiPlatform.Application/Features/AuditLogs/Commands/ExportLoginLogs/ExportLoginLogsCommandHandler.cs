using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Commands.ExportLoginLogs;

public class ExportLoginLogsCommandHandler
    : IRequestHandler<ExportLoginLogsCommand, Result<ExportFileResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IExportService _exportService;

    public ExportLoginLogsCommandHandler(
        IApplicationDbContext context,
        IExportService exportService)
    {
        _context = context;
        _exportService = exportService;
    }

    public async Task<Result<ExportFileResult>> Handle(
        ExportLoginLogsCommand request,
        CancellationToken cancellationToken)
    {
        // 构建查询
        var query = _context.LoginLogs.AsQueryable();

        if (!string.IsNullOrEmpty(request.UserName))
        {
            query = query.Where(l => l.UserName != null && l.UserName.Contains(request.UserName));
        }

        if (!string.IsNullOrEmpty(request.LoginType))
        {
            query = query.Where(l => l.LoginType == request.LoginType);
        }

        if (request.IsSuccess.HasValue)
        {
            query = query.Where(l => l.IsSuccess == request.IsSuccess.Value);
        }

        if (request.StartTime.HasValue)
        {
            var startInstant = NodaTime.Instant.FromDateTimeUtc(request.StartTime.Value.ToUniversalTime());
            query = query.Where(l => l.CreatedAt >= startInstant);
        }

        if (request.EndTime.HasValue)
        {
            var endInstant = NodaTime.Instant.FromDateTimeUtc(request.EndTime.Value.ToUniversalTime());
            query = query.Where(l => l.CreatedAt <= endInstant);
        }

        // 按时间降序排序
        query = query.OrderByDescending(l => l.CreatedAt);

        // 限制导出数量
        var maxExportCount = 10000;
        var data = await query
            .Take(maxExportCount)
            .Select(l => new LoginLogExportDto
            {
                UserName = l.UserName ?? "-",
                LoginType = l.LoginType,
                IsSuccess = l.IsSuccess,
                FailureReason = l.FailureReason ?? "-",
                IpAddress = l.IpAddress,
                Location = l.Location ?? "-",
                Browser = l.Browser ?? "-",
                OperatingSystem = l.OperatingSystem ?? "-",
                DeviceType = l.DeviceType ?? "-",
                CreatedAt = l.CreatedAt.ToDateTimeUtc()
            })
            .ToListAsync(cancellationToken);

        if (data.Count == 0)
        {
            return Result.Fail<ExportFileResult>("没有可导出的数据");
        }

        // 列映射
        var columnMappings = new Dictionary<string, string>
        {
            { nameof(LoginLogExportDto.UserName), "用户名" },
            { nameof(LoginLogExportDto.LoginType), "登录类型" },
            { nameof(LoginLogExportDto.IsSuccess), "是否成功" },
            { nameof(LoginLogExportDto.FailureReason), "失败原因" },
            { nameof(LoginLogExportDto.IpAddress), "IP地址" },
            { nameof(LoginLogExportDto.Location), "地理位置" },
            { nameof(LoginLogExportDto.Browser), "浏览器" },
            { nameof(LoginLogExportDto.OperatingSystem), "操作系统" },
            { nameof(LoginLogExportDto.DeviceType), "设备类型" },
            { nameof(LoginLogExportDto.CreatedAt), "创建时间" }
        };

        // 导出文件
        byte[] fileContent;
        string fileName;
        string contentType;

        if (request.Format.ToLower() == "csv")
        {
            fileContent = await _exportService.ExportToCsvAsync(
                data,
                columnMappings,
                cancellationToken);

            fileName = $"登录日志_{DateTime.Now:yyyyMMddHHmmss}.csv";
            contentType = "text/csv";
        }
        else
        {
            fileContent = await _exportService.ExportToExcelAsync(
                data,
                "登录日志",
                columnMappings,
                cancellationToken);

            fileName = $"登录日志_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        var result = new ExportFileResult
        {
            FileContent = fileContent,
            FileName = fileName,
            ContentType = contentType
        };

        return Result.Ok(result);
    }
}

/// <summary>
/// 登录日志导出 DTO
/// </summary>
public class LoginLogExportDto
{
    public string UserName { get; set; } = null!;
    public string LoginType { get; set; } = null!;
    public bool IsSuccess { get; set; }
    public string FailureReason { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string Browser { get; set; } = null!;
    public string OperatingSystem { get; set; } = null!;
    public string DeviceType { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
