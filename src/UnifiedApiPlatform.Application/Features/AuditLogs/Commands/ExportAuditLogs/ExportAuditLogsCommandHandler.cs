using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UnifiedApiPlatform.Application.Common.Interfaces;
using UnifiedApiPlatform.Application.Common.Models;

namespace UnifiedApiPlatform.Application.Features.AuditLogs.Commands.ExportAuditLogs;

public class ExportAuditLogsCommandHandler
    : IRequestHandler<ExportAuditLogsCommand, Result<ExportFileResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IExportService _exportService;

    public ExportAuditLogsCommandHandler(
        IApplicationDbContext context,
        IExportService exportService)
    {
        _context = context;
        _exportService = exportService;
    }

    public async Task<Result<ExportFileResult>> Handle(
        ExportAuditLogsCommand request,
        CancellationToken cancellationToken)
    {
        // 构建查询（与 GetAuditLogsQuery 相同的筛选逻辑）
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(request.UserName))
        {
            query = query.Where(a => a.UserName != null && a.UserName.Contains(request.UserName));
        }

        if (!string.IsNullOrEmpty(request.Action))
        {
            query = query.Where(a => a.Action == request.Action);
        }

        if (!string.IsNullOrEmpty(request.EntityType))
        {
            query = query.Where(a => a.EntityType == request.EntityType);
        }

        if (!string.IsNullOrEmpty(request.HttpMethod))
        {
            query = query.Where(a => a.HttpMethod == request.HttpMethod);
        }

        if (request.IsSuccess.HasValue)
        {
            query = query.Where(a => a.IsSuccess == request.IsSuccess.Value);
        }

        if (request.StartTime.HasValue)
        {
            var startInstant = NodaTime.Instant.FromDateTimeUtc(request.StartTime.Value.ToUniversalTime());
            query = query.Where(a => a.CreatedAt >= startInstant);
        }

        if (request.EndTime.HasValue)
        {
            var endInstant = NodaTime.Instant.FromDateTimeUtc(request.EndTime.Value.ToUniversalTime());
            query = query.Where(a => a.CreatedAt <= endInstant);
        }

        // 按时间降序排序
        query = query.OrderByDescending(a => a.CreatedAt);

        // 限制导出数量（避免内存溢出）
        var maxExportCount = 10000;
        var data = await query
            .Take(maxExportCount)
            .Select(a => new AuditLogExportDto
            {
                UserName = a.UserName ?? "-",
                Action = a.Action,
                EntityType = a.EntityType ?? "-",
                EntityId = a.EntityId ?? "-",
                HttpMethod = a.HttpMethod,
                RequestPath = a.RequestPath,
                StatusCode = a.StatusCode,
                Duration = a.Duration,
                IpAddress = a.IpAddress,
                IsSuccess = a.IsSuccess,
                CreatedAt = a.CreatedAt.ToDateTimeUtc()
            })
            .ToListAsync(cancellationToken);

        if (data.Count == 0)
        {
            return Result.Fail<ExportFileResult>("没有可导出的数据");
        }

        // 列映射
        var columnMappings = new Dictionary<string, string>
        {
            { nameof(AuditLogExportDto.UserName), "用户名" },
            { nameof(AuditLogExportDto.Action), "操作类型" },
            { nameof(AuditLogExportDto.EntityType), "实体类型" },
            { nameof(AuditLogExportDto.EntityId), "实体ID" },
            { nameof(AuditLogExportDto.HttpMethod), "HTTP方法" },
            { nameof(AuditLogExportDto.RequestPath), "请求路径" },
            { nameof(AuditLogExportDto.StatusCode), "状态码" },
            { nameof(AuditLogExportDto.Duration), "执行时间(ms)" },
            { nameof(AuditLogExportDto.IpAddress), "IP地址" },
            { nameof(AuditLogExportDto.IsSuccess), "是否成功" },
            { nameof(AuditLogExportDto.CreatedAt), "创建时间" }
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

            fileName = $"操作日志_{DateTime.Now:yyyyMMddHHmmss}.csv";
            contentType = "text/csv";
        }
        else
        {
            fileContent = await _exportService.ExportToExcelAsync(
                data,
                "操作日志",
                columnMappings,
                cancellationToken);

            fileName = $"操作日志_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
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
/// 操作日志导出 DTO
/// </summary>
public class AuditLogExportDto
{
    public string UserName { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string HttpMethod { get; set; } = null!;
    public string RequestPath { get; set; } = null!;
    public int StatusCode { get; set; }
    public long Duration { get; set; }
    public string IpAddress { get; set; } = null!;
    public bool IsSuccess { get; set; }
    public DateTime CreatedAt { get; set; }
}
