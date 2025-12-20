namespace ChmlFrp.SDK.Responses;

/// <summary>
/// 创建/修改隧道响应
/// </summary>
public class CreateTunnelResponse : BaseResponse
{
    /// <summary>
    /// 隧道数据
    /// </summary>
    [JsonPropertyName("data")]
    public TunnelData? Data { get; set; }
}