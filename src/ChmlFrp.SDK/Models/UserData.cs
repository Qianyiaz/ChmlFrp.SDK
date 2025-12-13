namespace ChmlFrp.SDK.Models;

/// <summary>
/// 用户数据
/// </summary>
public class UserData
{
    /// <summary>
    /// 用户的唯一标识ID
    /// </summary>
    [JsonPropertyName("id")]
    public int UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// 用户身份验证令牌
    /// </summary>
    [JsonPropertyName("usertoken")]
    public string? UserToken { get; set; }

    /// <summary>
    /// 用户邮箱地址
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// 用户的QQ号码
    /// </summary>
    [JsonPropertyName("qq")]
    public string? QQNumber { get; set; }

    /// <summary>
    /// 用户所属组别(等级/会员类型)
    /// </summary>
    [JsonPropertyName("usergroup")]
    public string? UserGroup { get; set; }

    /// <summary>
    /// 用户组到期时间
    /// </summary>
    [JsonPropertyName("term")]
    public string? MembershipExpiry { get; set; }

    /// <summary>
    /// 实名认证状态
    /// </summary>
    [JsonPropertyName("realname")]
    public string? RealNameStatus { get; set; }

    /// <summary>
    /// 用户隧道数
    /// </summary>
    [JsonPropertyName("tunnelCount")]
    public int CreativeTunnelCount { get; set; }

    /// <summary>
    /// 用户隧道上限
    /// </summary>
    [JsonPropertyName("tunnel")]
    public int MaxTunnelLimit { get; set; }

    /// <summary>
    /// 剩余可用隧道槽位数量
    /// </summary>
    [JsonIgnore]
    public int AvailableTunnelSlots => MaxTunnelLimit - CreativeTunnelCount;

    /// <summary>
    /// 用户所有隧道的当前连接总数
    /// </summary>
    [JsonPropertyName("totalCurConns")]
    public int CurrentConnectionCount { get; set; }

    /// <summary>
    /// 累计上传数据量(字节)
    /// </summary>
    [JsonPropertyName("total_upload")]
    public long TotalUploadBytes { get; set; }

    /// <summary>
    /// 累计下载数据量(字节)
    /// </summary>
    [JsonPropertyName("total_download")]
    public long TotalDownloadBytes { get; set; }

    /// <summary>
    /// 国内带宽限制(Mbps)
    /// </summary>
    [JsonPropertyName("bandwidth")]
    public int DomesticBandwidth { get; set; }

    /// <summary>
    /// 国外带宽限制国(内带宽的4倍)(Mbps)
    /// </summary>
    [JsonIgnore]
    public int InternationalBandwidth => DomesticBandwidth * 4;

    /// <summary>
    /// 用户积分
    /// </summary>
    [JsonPropertyName("integral")]
    public int CreditPoints { get; set; }

    /// <summary>
    /// 用户头像的URL地址
    /// </summary>
    [JsonPropertyName("userimg")]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 用户注册时间
    /// </summary>
    [JsonPropertyName("regtime")]
    public string? RegistrationDate { get; set; }

    /// <summary>
    /// 累计上传数据量(MB)
    /// </summary>
    [JsonIgnore]
    public double TotalUploadMB => TotalUploadBytes / 1024.0 / 1024.0;

    /// <summary>
    /// 累计下载数据量(MB)
    /// </summary>
    [JsonIgnore]
    public double TotalDownloadMB => TotalDownloadBytes / 1024.0 / 1024.0;

    /// <summary>
    /// 累计上传数据量(GB)
    /// </summary>
    [JsonIgnore]
    public double TotalUploadGB => TotalUploadMB / 1024.0;

    /// <summary>
    /// 累计下载数据量(GB)
    /// </summary>
    [JsonIgnore]
    public double TotalDownloadGB => TotalDownloadMB / 1024.0;
}