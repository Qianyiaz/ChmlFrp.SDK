namespace ChmlFrp.SDK.Models;

/// <summary>
/// 存储用户令牌的类
/// </summary>
public class JsonData
{
    /// <summary>
    /// 用户令牌
    /// </summary>
    [JsonPropertyName("usertoken")]
    public string? UserToken { get; set; }
}