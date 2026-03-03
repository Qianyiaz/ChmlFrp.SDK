namespace ChmlFrp.SDK.Models;

/// <summary>
/// 表示节点的状态数据。
/// </summary>
public class NodeStateData
{
    /// <summary>
    /// 获取或设置节点总入站流量(bytes)
    /// </summary>
    [JsonPropertyName("total_traffic_in")]
    public long TotalTrafficIn { get; set; }

    /// <summary>
    /// 获取或设置节点总出站流量(bytes)
    /// </summary>
    [JsonPropertyName("total_traffic_out")]
    public long TotalTrafficOut { get; set; }

    /// <summary>
    /// 获取或设置当前连接数
    /// </summary>
    [JsonPropertyName("cur_counts")]
    public int CurCounts { get; set; }

    /// <summary>
    /// 获取或设置节点名称
    /// </summary>
    [JsonPropertyName("node_name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 获取或设置节点 ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 节点状态的字符串
    /// </summary>
    [JsonPropertyName("state")]
    public string StateString { get; set; } = null!;

    /// <summary>
    /// 节点是否在线
    /// </summary>
    [JsonIgnore]
    public bool State => StateString == "online";

    /// <summary>
    /// 带宽使用百分比
    /// </summary>
    [JsonPropertyName("bandwidth_usage_percent")]
    public int BandwidthUsagePercent { get; set; }

    /// <summary>
    /// 当前上传带宽使用百分比
    /// </summary>
    [JsonPropertyName("current_upload_usage_percent")]
    public int CurrentUploadUsagePercent { get; set; }

    /// <summary>
    /// CPU 使用率
    /// </summary>
    [JsonPropertyName("cpu_usage")]
    public float CpuUsage { get; set; }

    /// <summary>
    /// 节点所属组
    /// </summary>
    [JsonPropertyName("nodegroup")]
    public string NodeGroup { get; set; } = null!;

    /// <summary>
    /// 连接的客户端数量
    /// </summary>
    [JsonPropertyName("client_counts")]
    public int ClientCounts { get; set; }

    /// <summary>
    /// 隧道数量
    /// </summary>
    [JsonPropertyName("tunnel_counts")]
    public int TunnelCounts { get; set; }
}