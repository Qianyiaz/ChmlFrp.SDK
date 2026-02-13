namespace ChmlFrp.SDK.Models;

/// <summary>
/// 创建隧道请求数据
/// </summary>
public class CreateTunnelRequest
{
    /// <summary>
    /// 隧道名称
    /// </summary>
    [JsonPropertyName("tunnelname")]
    public string? TunnelName { get; set; }

    /// <summary>
    /// 隧道所属节点名称
    /// </summary>
    [JsonPropertyName("node")]
    public string? Node { get; set; }

    /// <summary>
    /// 隧道类型字符串 tcp/udp/http/https
    /// </summary>
    [JsonPropertyName("porttype")]
    public string? PortType { get; set; }

    /// <summary>
    /// 本地IP地址
    /// </summary>
    [JsonPropertyName("localip")]
    public string? LocalIp { get; set; }

    /// <summary>
    /// 本地端口
    /// </summary>
    [JsonPropertyName("localport")]
    public int LocalPort { get; set; }

    /// <summary>
    /// 远程端口或域名(根据隧道类型不同而不同)
    /// </summary>
    [JsonPropertyName("remoteport")]
    public int RemotePort { get; set; }

    /// <summary>
    /// 远程端口或域名(根据隧道类型不同而不同)
    /// </summary>
    [JsonPropertyName("banddomain")]
    public string? BandDomain { get; set; }

    /// <summary>
    /// 是否启用加密
    /// </summary>
    [JsonPropertyName("encryption")]
    public bool Encryption { get; set; }

    /// <summary>
    /// 是否启用压缩
    /// </summary>
    [JsonPropertyName("compression")]
    public bool Compression { get; set; }

    /// <summary>
    /// frp 的额外参数字符串(extraparams)
    /// </summary>
    [JsonPropertyName("extraparams")]
    public string? ExtraParams { get; set; }
}