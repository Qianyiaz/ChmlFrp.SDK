namespace ChmlFrp.SDK.Models;

public class CreateTunnelRequest
{
    [JsonPropertyName("tunnelname")]
    public string? TunnelName { get; set; }

    [JsonPropertyName("node")]
    public string? Node { get; set; }

    /// <summary>tcp/udp/http/https</summary>
    [JsonPropertyName("porttype")]
    public string? PortType { get; set; }

    [JsonPropertyName("localip")]
    public string? LocalIp { get; set; }

    [JsonPropertyName("localport")]
    public int? LocalPort { get; set; }

    [JsonPropertyName("remoteport")]
    public int? RemotePort { get; set; }

    [JsonPropertyName("banddomain")]
    public string? BandDomain { get; set; }

    [JsonPropertyName("encryption")]
    public bool? Encryption { get; set; }

    [JsonPropertyName("compression")]
    public bool? Compression { get; set; }

    /// <summary>
    /// frp 的额外参数字符串（extraparams）
    /// </summary>
    [JsonPropertyName("extraparams")]
    public string? ExtraParams { get; set; }
}