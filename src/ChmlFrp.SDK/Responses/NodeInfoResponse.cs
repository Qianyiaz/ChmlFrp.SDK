namespace ChmlFrp.SDK.Responses;

/// <summary>
/// 节点详情请求
/// </summary>
public class NodeInfoResponse : BaseResponse
{
    /// <summary>
    /// 节点详情
    /// </summary>
    [JsonPropertyName("data")]
    public NodeInfo? Data { get; set; }
}