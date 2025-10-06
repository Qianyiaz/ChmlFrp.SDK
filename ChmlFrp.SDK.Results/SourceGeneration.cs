using ChmlFrp.SDK.Results;

namespace ChmlFrp.SDK;

/// <summary>
///     源生成器,用于支持Aot.
/// </summary>
[JsonSerializable(typeof(TokenData))]
[JsonSerializable(typeof(BaseResult))]
[JsonSerializable(typeof(NodeResult))]
[JsonSerializable(typeof(UserResult))]
[JsonSerializable(typeof(TunnelResult))]
public partial class SourceGeneration : JsonSerializerContext;