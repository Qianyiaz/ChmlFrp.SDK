namespace ChmlFrp.SDK.Results;

public class NodeResult : BaseResult
{
    /// <summary>
    ///     节点数据
    /// </summary>
    [JsonPropertyName("data")]
    public List<NodeData> Data { get; set; }
}

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class NodeData
{
    /// <summary>
    ///     隧道的唯一标识ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     隧道名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    ///     节点地区
    /// </summary>
    [JsonPropertyName("area")]
    public string Area { get; set; }

    /// <summary>
    ///     节点权限组
    /// </summary>
    [JsonPropertyName("nodegroup")]
    public string NodeGroup { get; set; }

    /// <summary>
    ///     带宽限速是否为中国(字符串)
    /// </summary>
    [JsonPropertyName("china")]
    public string ChinaString { get; set; }

    /// <summary>
    ///     带宽限速是否为中国
    /// </summary>
    [JsonIgnore]
    public bool IsInChina => ChinaString == "yes";

    /// <summary>
    ///     是否允许建站(字符串)
    /// </summary>
    [JsonPropertyName("web")]
    public string WebString { get; set; }

    /// <summary>
    ///     是否允许建站(字符串)
    /// </summary>
    [JsonIgnore]
    public bool IsWeb => WebString == "yes";

    /// <summary>
    ///     是否允许UDP(字符串)
    /// </summary>
    [JsonPropertyName("udp")]
    public string SupportsUdpString { get; set; }

    /// <summary>
    ///     是否允许UDP
    /// </summary>
    [JsonIgnore]
    public bool SupportsUdp => SupportsUdpString == "true";

    /// <summary>
    ///     是否有防御(字符串)
    /// </summary>
    [JsonPropertyName("fangyu")]
    public string HasDefenseString { get; set; }

    /// <summary>
    ///     是否有防御(字符串)
    /// </summary>
    [JsonIgnore]
    public bool HasDefense => HasDefenseString == "true";

    /// <summary>
    ///     介绍
    /// </summary>
    [JsonPropertyName("notes")]
    public string Notes { get; set; }
}