using System.Text.Json.Serialization;

namespace ConsoleJson.Forecast;

public class GetForecast
{
    /// <summary>
    ///     返回消息
    /// </summary>
    [JsonPropertyName("msg")]
    public string Message { get; set; }

    /// <summary>
    ///     请求是否成功(字符串)
    /// </summary>
    [JsonPropertyName("state")]
    public string StateString { get; set; }

    /// <summary>
    ///     请求是否成功
    /// </summary>
    [JsonIgnore]
    // ReSharper disable once UnusedMember.Global
    public bool State => StateString == "success";
}

[JsonSerializable(typeof(GetForecast))]
public partial class GetForecastContext : JsonSerializerContext;