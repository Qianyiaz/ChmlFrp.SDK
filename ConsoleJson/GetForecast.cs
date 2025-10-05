using System.Text.Json.Serialization;

namespace ConsoleJson;

public class GetForecast
{
    [JsonPropertyName("msg")]
    public string Message { get; set; }
}

[JsonSerializable(typeof(GetForecast))]
[JsonSerializable(typeof(string))]
public partial class GetForecastContext : JsonSerializerContext;