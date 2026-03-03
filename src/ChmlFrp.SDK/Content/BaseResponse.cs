namespace ChmlFrp.SDK.Content;

/// <summary>
/// 基本请求
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// 返回消息
    /// </summary>
    [JsonPropertyName("msg")]
    public string Message { get; set; } = null!;

    /// <summary>
    /// 请求返回 Code
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// 请求是否成功
    /// </summary>
    [JsonIgnore]
    public bool State => Code == 200;
}