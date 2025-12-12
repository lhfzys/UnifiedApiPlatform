using System.Reflection;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using UnifiedApiPlatform.Application.Common.Interfaces;

namespace UnifiedApiPlatform.Infrastructure.Services;

/// <summary>
/// 导出服务实现
/// </summary>
public class ExportService : IExportService
{
    public ExportService()
    {
        ExcelPackage.License.SetNonCommercialPersonal("Tayor");
    }

    public async Task<byte[]> ExportToExcelAsync<T>(
        List<T> data,
        string sheetName,
        Dictionary<string, string> columnMappings,
        CancellationToken cancellationToken = default)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        // 写入表头
        var columnIndex = 1;
        var propertyColumnMap = new Dictionary<string, int>();

        foreach (var mapping in columnMappings)
        {
            worksheet.Cells[1, columnIndex].Value = mapping.Value;
            propertyColumnMap[mapping.Key] = columnIndex;
            columnIndex++;
        }

        // 设置表头样式
        using (var headerRange = worksheet.Cells[1, 1, 1, columnMappings.Count])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        // 写入数据
        var rowIndex = 2;
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var item in data)
        {
            foreach (var mapping in columnMappings)
            {
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(mapping.Key, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    var value = property.GetValue(item);
                    var cellIndex = propertyColumnMap[mapping.Key];

                    // 格式化值
                    var formattedValue = FormatValue(value);
                    worksheet.Cells[rowIndex, cellIndex].Value = formattedValue;
                }
            }

            rowIndex++;
        }

        // 自动调整列宽
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // 设置数据区域边框
        if (data.Count > 0)
        {
            using var dataRange = worksheet.Cells[1, 1, rowIndex - 1, columnMappings.Count];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        return await package.GetAsByteArrayAsync(cancellationToken);
    }

    public Task<byte[]> ExportToCsvAsync<T>(
        List<T> data,
        Dictionary<string, string> columnMappings,
        CancellationToken cancellationToken = default)
    {
        var csv = new StringBuilder();

        // 写入表头
        csv.AppendLine(string.Join(",", columnMappings.Values.Select(EscapeCsvValue)));

        // 写入数据
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var item in data)
        {
            var values = new List<string>();

            foreach (var mapping in columnMappings)
            {
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(mapping.Key, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    var value = property.GetValue(item);
                    var formattedValue = FormatValue(value);
                    values.Add(EscapeCsvValue(formattedValue?.ToString()));
                }
                else
                {
                    values.Add(string.Empty);
                }
            }

            csv.AppendLine(string.Join(",", values));
        }

        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv.ToString()))
            .ToArray();

        return Task.FromResult(bytes);
    }

    /// <summary>
    /// 格式化值
    /// </summary>
    private static object? FormatValue(object? value)
    {
        if (value == null)
            return null;

        return value switch
        {
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
            DateTimeOffset dto => dto.ToString("yyyy-MM-dd HH:mm:ss"),
            bool b => b ? "是" : "否",
            _ => value
        };
    }

    /// <summary>
    /// 转义 CSV 值
    /// </summary>
    private static string EscapeCsvValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        // 如果包含逗号、引号或换行符，需要用引号包裹
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            // 引号需要转义为两个引号
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
