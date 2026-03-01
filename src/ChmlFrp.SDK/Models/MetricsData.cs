namespace ChmlFrp.SDK.Models;

/// <summary>
/// 服务器具体负载数据
/// </summary>
public class MetricsData
{
    /// <summary>
    /// CPU负载百分比
    /// </summary>
    [JsonPropertyName("cpu")]
    public float CpuLoad { get; set; }

    /// <summary>
    /// 内存负载百分比
    /// </summary>
    [JsonPropertyName("memory")]
    public float MemoryLoad { get; set; }

    /// <summary>
    /// 磁盘负载百分比
    /// </summary>
    [JsonPropertyName("steal")]
    public float Steal { get; set; }

    /// <summary>
    /// 网络负载百分比
    /// </summary>
    [JsonPropertyName("ioLatency")]
    public float IoLatency { get; set; }

    /// <summary>
    /// 线程争用负载百分比
    /// </summary>
    [JsonPropertyName("threadContention")]
    public float ThreadContention { get; set; }
}