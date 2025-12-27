namespace ChmlFrp.SDK.Content;

/// <summary>
/// 数据响应结构
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public sealed class DataResponse<T> : BaseResponse
{
    /// <summary>
    /// 数据
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}