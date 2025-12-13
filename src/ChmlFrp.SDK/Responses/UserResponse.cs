namespace ChmlFrp.SDK.Responses;

/// <summary>
/// 用户请求
/// </summary>
public class UserResponse : BaseResponse
{
    /// <summary>
    /// 用户数据
    /// </summary>
    [JsonPropertyName("data")]
    public UserData? Data { get; set; }
}