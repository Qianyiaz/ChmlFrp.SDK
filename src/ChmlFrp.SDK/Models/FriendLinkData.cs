namespace ChmlFrp.SDK.Models;

/// <summary>
/// 友链,就是在官网底部的友情链接
/// </summary>
public sealed class FriendLinkData
{
    /// <summary>
    /// 名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 介绍
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    /// <summary>
    /// 链接
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
}