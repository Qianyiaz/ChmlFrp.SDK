namespace ChmlFrp.SDK.Responses;

/// <summary>
/// 基本请求
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// 返回消息
    /// </summary>
    [JsonPropertyName("msg")]
    public string? Message { get; set; }

    /// <summary>
    /// 请求是否成功(字符串)
    /// </summary>
    [JsonPropertyName("state")]
    public string? StateString { get; set; }

    /// <summary>
    /// 请求是否成功
    /// </summary>
    [JsonIgnore]
    public bool State => StateString == "success";
}