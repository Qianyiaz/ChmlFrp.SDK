namespace ChmlFrp.SDK.Models;

/// <summary>
/// 面板数据
/// </summary>
public class PanelInfo
{
    /// <summary>
    /// 隧道数
    /// </summary>
    [JsonPropertyName("tunnel_amount")]
    public int TunnelAmount { get; set; }

    /// <summary>
    /// 用户数
    /// </summary>
    [JsonPropertyName("user_amount")]
    public int UserAmount { get; set; }

    /// <summary>
    /// 节点数
    /// </summary>
    [JsonPropertyName("node_amount")]
    public int NodeAmount { get; set; }

    /// <summary>
    /// 友链
    /// </summary>
    [JsonPropertyName("friend_links")]
    public IReadOnlyList<FriendLinkData> FriendLinks { get; set; } = null!;
}