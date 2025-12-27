namespace ChmlFrp.SDK.Models;

/// <summary>
/// 创建隧道请求模型
/// </summary>
public class CreateTunnelRequest
{
    /// <summary>
    /// 用户令牌
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// 隧道名称
    /// </summary>
    [JsonPropertyName("tunnelname")]
    public string? TunnelName { get; set; }

    /// <summary>
    /// 节点名称
    /// </summary>
    [JsonPropertyName("node")]
    public string? Node { get; set; }

    /// <summary>
    /// 隧道类型
    /// </summary>
    [JsonPropertyName("porttype")]
    public string? PortType { get; set; }

    /// <summary>
    /// 本地IP
    /// </summary>
    [JsonPropertyName("localip")]
    public string? LocalIp { get; set; } = "127.0.0.1";

    /// <summary>
    /// 本地端口
    /// </summary>
    [JsonPropertyName("localport")]
    public int LocalPort { get; set; }

    /// <summary>
    /// 远程端口
    /// </summary>
    [JsonPropertyName("remoteport")]
    public int? RemotePort { get; set; }

    /// <summary>
    /// 绑定域名
    /// </summary>
    [JsonPropertyName("banddomain")]
    public string? BandDomain { get; set; }

    /// <summary>
    /// 是否数据加密
    /// </summary>
    [JsonPropertyName("encryption")]
    public bool Encryption { get; set; }

    /// <summary>
    /// 是否数据压缩
    /// </summary>
    [JsonPropertyName("compression")]
    public bool Compression { get; set; }

    /// <summary>
    /// frp额外参数
    /// </summary>
    [JsonPropertyName("extraparams")]
    public string? ExtraParams { get; set; } = "";
}