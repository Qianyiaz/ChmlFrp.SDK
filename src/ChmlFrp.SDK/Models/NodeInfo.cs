namespace ChmlFrp.SDK.Models;

/// <summary>
/// 节点详情
/// </summary>
public class NodeInfo : NodeData
{
    /// <summary>
    /// 在线状态(字符串)
    /// </summary>
    [JsonPropertyName("state")]
    public string? StateString { get; set; }

    /// <summary>
    /// 在线状态
    /// </summary>
    [JsonIgnore]
    public bool State => StateString == "online";

    /// <summary>
    /// 节点IP地址
    /// </summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>
    /// 节点的真实IP
    /// </summary>
    [JsonPropertyName("realIp")]
    public string? RealIp { get; set; }

    /// <summary>
    /// 节点FRP端口
    /// </summary>
    [JsonPropertyName("port")]
    public int LocalPort { get; set; }

    /// <summary>
    /// 节点FRP端口限制
    /// </summary>
    [JsonPropertyName("rport")]
    public string? RemoteEndpoint { get; set; }

    /// <summary>
    /// CPU信息
    /// </summary>
    [JsonPropertyName("cpu_info")]
    public string? CpuInfo { get; set; }

    /// <summary>
    /// CPU核心数
    /// </summary>
    [JsonPropertyName("num_cores")]
    public int NumCores { get; set; }

    /// <summary>
    /// 今日下载流量(bytes)
    /// </summary>
    [JsonPropertyName("total_traffic_in")]
    public long TotalTrafficIn { get; set; }

    /// <summary>
    /// 今日上传流量(bytes)
    /// </summary>
    [JsonPropertyName("total_traffic_out")]
    public long TotalTrafficOut { get; set; }

    /// <summary>
    /// 带宽占用百分比
    /// </summary>
    [JsonPropertyName("bandwidth_usage_percent")]
    public int BandwidthUsagePercent { get; set; }

    /// <summary>
    /// 总内存量(bytes)
    /// </summary>
    [JsonPropertyName("memory_total")]
    public long MemoryTotal { get; set; }

    /// <summary>
    /// 总存储量(bytes)
    /// </summary>
    [JsonPropertyName("storage_total")]
    public long StorageTotal { get; set; }

    /// <summary>
    /// 存储使用量(bytes)
    /// </summary>
    [JsonPropertyName("storage_used")]
    public long StorageUsed { get; set; }

    /// <summary>
    /// 正常运行秒数
    /// </summary>
    [JsonPropertyName("uptime_seconds")]
    public long UptimeSeconds { get; set; }

    /// <summary>
    /// 正常运行小时
    /// </summary>
    [JsonIgnore]
    public double UptimeHours => UptimeSeconds / 60.0 / 60.0;

    /// <summary>
    /// 正常运行小时
    /// </summary>
    [JsonIgnore]
    public double UptimeDays => UptimeHours / 24.0;

    /// <summary>
    /// 1分钟平均负载
    /// </summary>
    [JsonPropertyName("load1")]
    public double Load1 { get; set; }

    /// <summary>
    /// 5分钟平均负载
    /// </summary>
    [JsonPropertyName("load5")]
    public double Load5 { get; set; }

    /// <summary>
    /// 15分钟平均负载
    /// </summary>
    [JsonPropertyName("load15")]
    public double Load15 { get; set; }

    /// <summary>
    /// 今日下载流量(GB)
    /// </summary>
    [JsonIgnore]
    public double TotalTrafficInGB => TotalTrafficIn / 1024.0 / 1024.0 / 1024.0;

    /// <summary>
    /// 今日上传流量(GB)
    /// </summary>
    [JsonIgnore]
    public double TotalTrafficOutGB => TotalTrafficOut / 1024.0 / 1024.0 / 1024.0;

    /// <summary>
    /// 总内存量(GB)
    /// </summary>
    [JsonIgnore]
    public double MemoryTotalGB => MemoryTotal / 1024.0 / 1024.0 / 1024.0;

    /// <summary>
    /// 总存储量(GB)
    /// </summary>
    [JsonIgnore]
    public double StorageTotalGB => StorageTotal / 1024.0 / 1024.0 / 1024.0;

    /// <summary>
    /// 存储使用量(GB)
    /// </summary>
    [JsonIgnore]
    public double StorageUsedGB => StorageUsed / 1024.0 / 1024.0 / 1024.0;

    /// <summary>
    /// 存储可用量(GB)
    /// </summary>
    [JsonIgnore]
    public double StorageAvailableGB => StorageTotalGB - StorageUsedGB;
}