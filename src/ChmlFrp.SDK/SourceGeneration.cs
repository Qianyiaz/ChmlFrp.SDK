using ChmlFrp.SDK.Responses;

namespace ChmlFrp.SDK;

/// <summary>
/// 源生成器,用于支持Aot.
/// </summary>
[JsonSerializable(typeof(NodeResponse))]
[JsonSerializable(typeof(BaseResponse))]
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(TunnelResponse))]
[JsonSerializable(typeof(NodeInfoResponse))]
[JsonSerializable(typeof(CreateTunnelRequest))]
[JsonSerializable(typeof(CreateTunnelResponse))]
public partial class SourceGeneration : JsonSerializerContext;