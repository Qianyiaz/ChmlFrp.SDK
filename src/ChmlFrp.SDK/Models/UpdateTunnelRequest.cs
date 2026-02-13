namespace ChmlFrp.SDK.Models;

/// <summary>
/// 更新隧道请求数据
/// </summary>
public class UpdateTunnelRequest : CreateTunnelRequest
{
    /// <summary>
    /// 隧道ID
    /// </summary>
    [JsonPropertyName("tunnelid")]
    public int TunnelId { get; set; }
}