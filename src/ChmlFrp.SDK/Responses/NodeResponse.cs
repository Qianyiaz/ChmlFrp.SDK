namespace ChmlFrp.SDK.Responses;

/// <summary>
/// 节点请求
/// </summary>
public class NodeResponse : BaseResponse
{
    /// <summary>
    /// 节点数据列表
    /// </summary>
    [JsonPropertyName("data")]
    public IReadOnlyList<NodeData>? Data { get; set; }
}