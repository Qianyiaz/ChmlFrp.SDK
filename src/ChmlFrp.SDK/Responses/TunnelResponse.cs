using System.Collections.Generic;

namespace ChmlFrp.SDK.Responses;

/// <summary>
/// 隧道请求
/// </summary>
public class TunnelResponse : BaseResponse
{
    /// <summary>
    /// 用户隧道数据
    /// </summary>
    [JsonPropertyName("data")]
    public IReadOnlyList<TunnelData>? Data { get; set; }
}