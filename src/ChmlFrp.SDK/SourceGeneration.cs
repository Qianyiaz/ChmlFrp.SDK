using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;

namespace ChmlFrp.SDK;

/// <inheritdoc cref="JsonSerializerContext" />
[JsonSerializable(typeof(BaseResponse))]
[JsonSerializable(typeof(CreateTunnelRequest))]
[JsonSerializable(typeof(UpdateTunnelRequest))]
[JsonSerializable(typeof(DataResponse<UserData>))]
[JsonSerializable(typeof(DataResponse<NodeInfo>))]
[JsonSerializable(typeof(DataResponse<TunnelData>))]
[JsonSerializable(typeof(DataResponse<IReadOnlyList<NodeData>>))]
[JsonSerializable(typeof(DataResponse<IReadOnlyList<TunnelData>>))]
public partial class SourceGeneration : JsonSerializerContext;