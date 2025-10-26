using System.Text.Json.Serialization;

namespace ChmlFrp.SDK.Services;

/// <summary>
///     源生成器,用于支持Aot.
/// </summary>
[JsonSerializable(typeof(NodeInfoResult))]
public partial class SourceGeneration : JsonSerializerContext;