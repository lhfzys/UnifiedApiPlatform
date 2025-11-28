using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnifiedApiPlatform.Shared.Helpers;

/// <summary>
/// JSON 帮助类
/// </summary>
public static class JsonHelper
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// 序列化为 JSON
    /// </summary>
    public static string Serialize<T>(T obj, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(obj, options ?? DefaultOptions);
    }

    /// <summary>
    /// 反序列化 JSON
    /// </summary>
    public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
    }

    /// <summary>
    /// 深度克隆
    /// </summary>
    public static T? Clone<T>(T obj)
    {
        var json = Serialize(obj);
        return Deserialize<T>(json);
    }
}
