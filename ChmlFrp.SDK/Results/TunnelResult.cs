using System.Diagnostics;

namespace ChmlFrp.SDK.Results;

/// <summary>
///     隧道请求
/// </summary>
public class TunnelResult : BaseResult
{
    /// <summary>
    ///     用户隧道数据
    /// </summary>
    [JsonPropertyName("data")]
    public List<TunnelData> Data { get; set; }
}

/// <summary>
///     隧道数据
/// </summary>
[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class TunnelData
{
    /// <summary>
    ///     隧道的唯一标识ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     隧道名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    ///     本地IP地址
    /// </summary>
    [JsonPropertyName("localip")]
    public string LocalIp { get; set; }

    /// <summary>
    ///     隧道类型(字符串)
    /// </summary>
    [JsonPropertyName("type")]
    public string TypeString { get; set; }

    /// <summary>
    ///     隧道类型枚举
    /// </summary>
    [JsonIgnore]
    public TunnelType Type => TypeString switch
    {
        "tcp" => TunnelType.Tcp,
        "udp" => TunnelType.Udp,
        "http" => TunnelType.Http,
        "https" => TunnelType.Https,
        _ => TunnelType.Tcp
    };

    /// <summary>
    ///     本地端口
    /// </summary>
    [JsonPropertyName("nport")]
    public int LocalPort { get; set; }

    /// <summary>
    ///     远程端口或域名
    /// </summary>
    [JsonPropertyName("dorp")]
    public string RemoteEndpoint { get; set; }

    /// <summary>
    ///     所属节点名称
    /// </summary>
    [JsonPropertyName("node")]
    public string NodeName { get; set; }

    /// <summary>
    ///     节点状态字符串
    /// </summary>
    [JsonPropertyName("nodestate")]
    public string NodeStateString { get; set; }

    /// <summary>
    ///     节点是否在线
    /// </summary>
    [JsonIgnore]
    public bool NodeState => NodeStateString == "online";

    /// <summary>
    ///     隧道状态字符串
    /// </summary>
    [JsonPropertyName("state")]
    public string StateString { get; set; }

    /// <summary>
    ///     隧道是否在线
    /// </summary>
    [JsonIgnore]
    public bool State => bool.Parse(StateString);

    /// <summary>
    ///     加密状态字符串
    /// </summary>
    [JsonPropertyName("encryption")]
    public string EncryptionString { get; set; }

    /// <summary>
    ///     是否启用加密
    /// </summary>
    [JsonIgnore]
    public bool IsEncrypted => bool.Parse(EncryptionString);

    /// <summary>
    ///     压缩状态字符串
    /// </summary>
    [JsonPropertyName("compression")]
    public string CompressionString { get; set; }

    /// <summary>
    ///     是否启用压缩
    /// </summary>
    [JsonIgnore]
    public bool IsCompressed => bool.Parse(CompressionString);

    /// <summary>
    ///     附加参数
    /// </summary>
    [JsonPropertyName("ap")]
    public string AdditionalParameters { get; set; }

    /// <summary>
    ///     今日上传流量(字节)
    /// </summary>
    [JsonPropertyName("today_traffic_in")]
    public long TodayUploadBytes { get; set; }

    /// <summary>
    ///     今日下载流量(字节)
    /// </summary>
    [JsonPropertyName("today_traffic_out")]
    public long TodayDownloadBytes { get; set; }

    /// <summary>
    ///     今日上传流量(MB)
    /// </summary>
    [JsonIgnore]
    public double TodayUploadMB =>  TodayUploadBytes / 1024.0 / 1024.0;

    /// <summary>
    ///     今日下载流量(MB)
    /// </summary>
    [JsonIgnore]
    public double TodayDownloadMB => TodayDownloadBytes / 1024.0 / 1024.0;

    /// <summary>
    ///     当前连接数
    /// </summary>
    [JsonPropertyName("cur_conns")]
    public int CurrentConnections { get; set; }

    /// <summary>
    ///     节点IP地址
    /// </summary>
    [JsonPropertyName("ip")]
    public string NodeIp { get; set; }

    /// <summary>
    ///     完整的远程地址
    /// </summary>
    [JsonIgnore]
    public string FullRemoteAddress => Type switch
    {
        TunnelType.Http => $"http://{NodeIp}{(RemoteEndpoint != "80" ? $":{RemoteEndpoint}" : "")}",
        TunnelType.Https => $"https://{NodeIp}{(RemoteEndpoint != "443" ? $":{RemoteEndpoint}" : "")}",
        TunnelType.Tcp or TunnelType.Udp => $"{NodeIp}:{RemoteEndpoint}",
        _ => throw new ArgumentOutOfRangeException()
    };

    /// <summary>
    ///     隧道类型
    /// </summary>
    public enum TunnelType
    {
        Tcp,
        Udp,
        Http,
        Https
    }

    /// <summary>
    ///     用于 ChmlFrp.SDK.Services
    /// </summary>
    [JsonIgnore]
    public Process FrpProcess { get; set; }

    /// <summary>
    ///     用于 ChmlFrp.SDK.Services
    /// </summary>
    [JsonIgnore]
    public bool IsRunning => FrpProcess is { HasExited: false };
}