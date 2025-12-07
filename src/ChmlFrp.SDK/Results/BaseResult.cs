using System.Net.Http;

namespace ChmlFrp.SDK.Results;

/// <summary>
/// 基本请求
/// </summary>
public class BaseResult
{
    /// <summary>
    /// 主HTTP客户端
    /// </summary>
    [JsonIgnore] 
    public static readonly HttpClient MainClient = new()
    {
        BaseAddress = new("https://cf-v2.uapis.cn/")
    };

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