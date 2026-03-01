namespace ChmlFrp.SDK.Models;

/// <summary>
/// 服务器负载数据
/// </summary>
public class ServerMetricsData
{
    /// <summary>
    /// 服务器名称
    /// </summary>
    [JsonPropertyName("serverName")]
    public string? Name { get; set; }

    /// <summary>
    /// 服务器负载百分比
    /// </summary>
    [JsonPropertyName("load")]
    public float Load { get; set; }

    /// <summary>
    /// 服务器具体负载数据
    /// </summary>
    [JsonPropertyName("metrics")]
    public MetricsData Metrics { get; set; }
}