using System.Text.Json;

namespace ChmlFrp.SDK.Results;

public class UserResult : BaseResult
{
    /// <summary>
    ///     用户数据
    /// </summary>
    [JsonPropertyName("data")]
    public UserData Data { get; set; }

    private static readonly string TokenFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChmlFrp", "user.json");

    /// <summary>
    ///     使用用户名和密码登录获取用户信息
    /// </summary>
    public static async Task<UserResult> LoginAsync(string username, string password)
    {
        HttpClientExtensions.MainClient ??= new HttpClient();

        try
        {
            var forecast = await HttpClientExtensions.MainClient.GetFromJsonAsync(
                $"https://cf-v2.uapis.cn/login?username={username}&password={password}",
                SourceGeneration.Default.UserResult
            );
            if (forecast.State)
                await SaveTokenAsync(forecast.Data.UserToken);
            return forecast;
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return new UserResult
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    ///     使用用户令牌获取用户信息
    /// </summary>
    public static async Task<UserResult> LoginByTokenAsync(string userToken)
    {
        HttpClientExtensions.MainClient ??= new HttpClient();

        try
        {
            var forecast = await HttpClientExtensions.MainClient.GetFromJsonAsync(
                $"https://cf-v2.uapis.cn/userinfo?token={userToken}",
                SourceGeneration.Default.UserResult
            );
            if (forecast.State)
                await SaveTokenAsync(forecast.Data.UserToken);
            return forecast;
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return new UserResult
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }

    private static async Task SaveTokenAsync(string userToken)
    {
        var directory = Path.GetDirectoryName(TokenFilePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);
        await File.WriteAllTextAsync(TokenFilePath, JsonSerializer.Serialize(new TokenData
        {
            UserToken = userToken
        }, SourceGeneration.Default.TokenData));
    }

    public static async Task<UserResult> AutoLogin()
    {
        if (!File.Exists(TokenFilePath))
            return new UserResult
            {
                StateString = "fail"
            };
        var json = await File.ReadAllTextAsync(TokenFilePath);
        var tokenData = JsonSerializer.Deserialize(json, SourceGeneration.Default.TokenData);
        return await LoginByTokenAsync(tokenData.UserToken);
    }
}

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class UserData
{
    /// <summary>
    ///     用户的唯一标识ID
    /// </summary>
    [JsonPropertyName("id")]
    public int UserId { get; set; }

    /// <summary>
    ///     用户名
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; set; }

    /// <summary>
    ///     用户身份验证令牌
    /// </summary>
    [JsonPropertyName("usertoken")]
    public string UserToken { get; set; }

    /// <summary>
    ///     用户邮箱地址
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; }

    /// <summary>
    ///     用户的QQ号码
    /// </summary>
    [JsonPropertyName("qq")]
    // ReSharper disable once InconsistentNaming
    public string QQNumber { get; set; }

    /// <summary>
    ///     用户所属组别（等级/会员类型）
    /// </summary>
    [JsonPropertyName("usergroup")]
    public string UserGroup { get; set; }

    /// <summary>
    ///     用户组到期时间
    /// </summary>
    [JsonPropertyName("term")]
    public string MembershipExpiry { get; set; }

    /// <summary>
    ///     实名认证状态
    /// </summary>
    [JsonPropertyName("realname")]
    public string RealNameStatus { get; set; }

    /// <summary>
    ///     用户隧道数
    /// </summary>
    [JsonPropertyName("tunnelCount")]
    public int CreativeTunnelCount { get; set; }

    /// <summary>
    ///     用户隧道上限
    /// </summary>
    [JsonPropertyName("tunnel")]
    public int MaxTunnelLimit { get; set; }

    /// <summary>
    ///     剩余可用隧道槽位数量
    /// </summary>
    [JsonIgnore]
    public int AvailableTunnelSlots => MaxTunnelLimit - CreativeTunnelCount;

    /// <summary>
    ///     用户所有隧道的当前连接总数
    /// </summary>
    [JsonPropertyName("totalCurConns")]
    public int CurrentConnectionCount { get; set; }

    /// <summary>
    ///     累计上传数据量（字节）
    /// </summary>
    [JsonPropertyName("total_upload")]
    public long TotalUploadBytes { get; set; }

    /// <summary>
    ///     累计下载数据量（字节）
    /// </summary>
    [JsonPropertyName("total_download")]
    public long TotalDownloadBytes { get; set; }

    /// <summary>
    ///     国内带宽限制（Mbps）
    /// </summary>
    [JsonPropertyName("bandwidth")]
    public int DomesticBandwidth { get; set; }

    /// <summary>
    ///     国外带宽限制（国内带宽的4倍）
    /// </summary>
    [JsonIgnore]
    public int InternationalBandwidth => DomesticBandwidth * 4;

    /// <summary>
    ///     用户积分
    /// </summary>
    [JsonPropertyName("integral")]
    public int CreditPoints { get; set; }

    /// <summary>
    ///     用户头像的URL地址
    /// </summary>
    [JsonPropertyName("userimg")]
    public string AvatarUrl { get; set; }

    /// <summary>
    ///     用户注册时间
    /// </summary>
    [JsonPropertyName("regtime")]
    public string RegistrationDate { get; set; }

    /// <summary>
    ///     累计上传数据量（MB）
    /// </summary>
    [JsonIgnore]
    // ReSharper disable once InconsistentNaming
    public double TotalUploadMB => TotalUploadBytes / 1024.0 / 1024.0;

    /// <summary>
    ///     累计下载数据量（MB）
    /// </summary>
    [JsonIgnore]
    // ReSharper disable once InconsistentNaming
    public double TotalDownloadMB => TotalDownloadBytes / 1024.0 / 1024.0;

    /// <summary>
    ///     累计上传数据量（GB）
    /// </summary>
    [JsonIgnore]
    // ReSharper disable once InconsistentNaming
    public double TotalUploadGB => TotalUploadBytes / 1024.0 / 1024.0 / 1024.0;

    /// <summary>
    ///     累计下载数据量（GB）
    /// </summary>
    [JsonIgnore]
    // ReSharper disable once InconsistentNaming
    public double TotalDownloadGB => TotalDownloadBytes / 1024.0 / 1024.0 / 1024.0;

    /// <summary>
    ///     获取用户的隧道列表
    /// </summary>
    public async Task<List<TunnelData>> GetTunnelsAsync()
    {
        HttpClientExtensions.MainClient ??= new HttpClient();

        TunnelResult result;
        try
        {
            result = await HttpClientExtensions.MainClient.GetFromJsonAsync(
                $"https://cf-v2.uapis.cn/tunnel?token={UserToken}",
                SourceGeneration.Default.TunnelResult
            );
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return [];
        }

        return result?.Data ?? [];
    }

    /// <summary>
    ///     获取节点列表
    /// </summary>
    public async Task<List<NodeData>> GetNodesAsync()
    {
        HttpClientExtensions.MainClient ??= new HttpClient();

        NodeResult result;
        try
        {
            result = await HttpClientExtensions.MainClient.GetFromJsonAsync(
                $"https://cf-v2.uapis.cn/node?token={UserToken}",
                SourceGeneration.Default.NodeResult
            );
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            return [];
        }

        return result?.Data ?? [];
    }
}

public class TokenData
{
    // ReSharper disable once StringLiteralTypo
    [JsonPropertyName("usertoken")] public string UserToken { get; set; }
}