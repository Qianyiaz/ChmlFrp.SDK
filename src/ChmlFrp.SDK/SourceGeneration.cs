namespace ChmlFrp.SDK;

/// <summary>
/// 源生成器,用于支持Aot.
/// </summary>
[JsonSerializable(typeof(JsonData))]
[JsonSerializable(typeof(NodeResult))]
[JsonSerializable(typeof(UserResult))]
[JsonSerializable(typeof(TunnelResult))]
[JsonSerializable(typeof(NodeInfoResult))]
public partial class SourceGeneration : JsonSerializerContext;