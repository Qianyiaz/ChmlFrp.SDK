namespace ChmlFrp.SDK.Models;

/// <summary>
/// 隧道数据
/// </summary>
public class TunnelData
{
    /// <summary>
    /// 隧道的唯一标识ID
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// 隧道名称
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 本地IP地址
    /// </summary>
    [JsonPropertyName("localip")]
    public string? LocalIp { get; set; }

    /// <summary>
    /// 隧道类型(字符串) tcp/udp/http/https
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 本地端口
    /// </summary>
    [JsonPropertyName("nport")]
    public int LocalPort { get; set; }

    /// <summary>
    /// 远程端口或域名
    /// </summary>
    [JsonPropertyName("dorp")]
    public string? RemoteEndpoint { get; set; }

    /// <summary>
    /// 所属节点名称
    /// </summary>
    [JsonPropertyName("node")]
    public string? NodeName { get; set; }

    /// <summary>
    /// 节点状态字符串
    /// </summary>
    [JsonPropertyName("nodestate")]
    public string? NodeStateString { get; set; }

    /// <summary>
    /// 节点是否在线
    /// </summary>
    [JsonIgnore]
    public bool NodeState => NodeStateString == "online";

    /// <summary>
    /// 隧道状态(字符串)
    /// </summary>
    [JsonPropertyName("state")]
    public string? StateString { get; set; }

    /// <summary>
    /// 隧道是否在线
    /// </summary>
    [JsonIgnore]
    public bool State => StateString != null && bool.Parse(StateString);

    /// <summary>
    ///  加密状态(字符串)
    /// </summary>
    [JsonPropertyName("encryption")]
    public string? EncryptionString { get; set; }

    /// <summary>
    /// 是否启用加密
    /// </summary>
    [JsonIgnore]
    public bool IsEncrypted => EncryptionString != null && bool.Parse(EncryptionString);

    /// <summary>
    /// 压缩状态(字符串)
    /// </summary>
    [JsonPropertyName("compression")]
    public string? CompressionString { get; set; }

    /// <summary>
    /// 是否启用压缩
    /// </summary>
    [JsonIgnore]
    public bool IsCompressed => CompressionString != null && bool.Parse(CompressionString);

    /// <summary>
    /// 附加参数
    /// </summary>
    [JsonPropertyName("ap")]
    public string? AdditionalParameters { get; set; }

    /// <summary>
    /// 今日上传流量(字节)
    /// </summary>
    [JsonPropertyName("today_traffic_in")]
    public long? TodayUploadBytes { get; set; }

    /// <summary>
    /// 今日下载流量(字节)
    /// </summary>
    [JsonPropertyName("today_traffic_out")]
    public long? TodayDownloadBytes { get; set; }

    /// <summary>
    /// 当前连接数
    /// </summary>
    [JsonPropertyName("cur_conns")]
    public int? CurrentConnections { get; set; }

    /// <summary>
    /// 节点IP地址
    /// </summary>
    [JsonPropertyName("ip")]
    public string? NodeIp { get; set; }

    private Lazy<string>? _fullRemoteAddress;

    /// <summary>
    /// 完整的远程地址
    /// </summary>
    [JsonIgnore]
    public string FullRemoteAddress
    {
        get
        {
            _fullRemoteAddress ??= new Lazy<string>(ComputeFullRemoteAddress);
            return _fullRemoteAddress.Value;
        }
    }

    private string ComputeFullRemoteAddress() => Type switch
    {
        "http" => $"http://{NodeIp}{(RemoteEndpoint != "80" ? $":{RemoteEndpoint}" : "")}",
        "https" => $"https://{NodeIp}{(RemoteEndpoint != "443" ? $":{RemoteEndpoint}" : "")}",
        "tcp" or "udp" => $"{NodeIp}:{RemoteEndpoint}",
        _ => throw new NotSupportedException($"Unsupported tunnel type: {Type}")
    };
}