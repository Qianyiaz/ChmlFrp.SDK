using ChmlFrp.SDK.Content;

namespace ChmlFrp.SDK;

/// <summary>
/// 源生成器,用于支持Aot.
/// </summary>
[JsonSerializable(typeof(BaseResponse))]
[JsonSerializable(typeof(CreateTunnelRequest))]
[JsonSerializable(typeof(DataResponse<UserData>))]
[JsonSerializable(typeof(DataResponse<NodeInfo>))]
[JsonSerializable(typeof(DataResponse<TunnelData>))]
[JsonSerializable(typeof(DataResponse<IReadOnlyList<NodeData>>))]
[JsonSerializable(typeof(DataResponse<IReadOnlyList<TunnelData>>))]
public partial class SourceGeneration : JsonSerializerContext;