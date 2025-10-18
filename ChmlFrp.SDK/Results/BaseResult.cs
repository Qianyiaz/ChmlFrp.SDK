using System.Reflection;

namespace ChmlFrp.SDK.Results;

/// <summary>
///     基本请求
/// </summary>
public class BaseResult
{
    /// <summary>
    ///     主HTTP客户端
    /// </summary>
    public static readonly HttpClient MainClient = new(new SocketsHttpHandler
    {
        UseProxy = false,
        EnableMultipleHttp2Connections = true
    })
    {
        BaseAddress = new Uri("https://cf-v2.uapis.cn"),
        DefaultRequestHeaders =
        {
            { "Accept", "application/json" },
            { "User-Agent", $"ChmlFrp.SDK/{Assembly.GetExecutingAssembly().GetName().Version}" }
        }
    };

    /// <summary>
    ///     返回消息
    /// </summary>
    [JsonPropertyName("msg")]
    public string Message { get; set; }

    /// <summary>
    ///     请求是否成功(字符串)
    /// </summary>
    [JsonPropertyName("state")]
    public string StateString { get; set; }

    /// <summary>
    ///     请求是否成功
    /// </summary>
    [JsonIgnore]
    public bool State => StateString == "success";

    /// <summary>
    ///     预热HTTP连接(在应用启动时调用)
    ///     可在启动时使用 _ = Task.Run(BaseResult.WarmUpConnectionAsync);
    /// </summary>
    public static async Task WarmUpConnectionAsync()
    {
        try
        {
            await MainClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/"));
        }
        catch
        {
            // ignored
        }
    }
}