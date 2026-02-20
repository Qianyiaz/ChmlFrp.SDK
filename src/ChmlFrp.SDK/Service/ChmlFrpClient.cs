using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;
using static ChmlFrp.SDK.SourceGeneration;

namespace ChmlFrp.SDK.Service;

/// <summary>
/// ChmlFrp Http 请求客户端
/// </summary>
public class ChmlFrpClient
{
    private readonly HttpClient _client;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="client">http客户端</param>
    public ChmlFrpClient(HttpClient? client = null) => _client = client ?? new()
    {
        BaseAddress = new("https://cf-v2.uapis.cn")
    };

    /// <summary>
    /// 判断是否登录
    /// </summary>
    /// <param name="tokenEscaped">用户token</param>
    /// <returns>是否登录</returns>
    public bool HasToken(out string tokenEscaped)
    {
        tokenEscaped = _client.DefaultRequestHeaders.Authorization!.ToString();
        return !string.IsNullOrWhiteSpace(tokenEscaped);
    }

    private static readonly string TokenFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChmlFrp", "user.json");

    private void SaveToken(string userToken)
    {
        var directory = Path.GetDirectoryName(TokenFilePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

        try
        {
            using var stream = File.Create(TokenFilePath);
            using var writer = new Utf8JsonWriter(stream);
            writer.WriteStartObject();
            writer.WriteString("usertoken", userToken);
            writer.WriteEndObject();
            writer.Flush();
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// 使用用户名和密码登录获取用户信息
    /// </summary>
    /// <param name="username">用户名用于登录</param>
    /// <param name="password">密码</param>
    /// <param name="saveToken">是否保存令牌</param>
    /// <returns>返回用户请求</returns>
    public async Task<DataResponse<UserData>?> LoginAsync(string? username, string? password, bool saveToken = true)
    {
        var forecast = await _client.GetFromJsonAsync($"login?username={username}&password={password}",
            Default.DataResponseUserData);
        if (forecast!.State)
        {
            var token = forecast.Data?.UserToken;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token!);
            if (saveToken)
                SaveToken(token!);
        }

        return forecast;
    }

    /// <summary>
    ///  使用用户令牌获取用户信息
    /// </summary>
    /// <param name="userToken">用户令牌用于登录</param>
    /// <param name="saveToken">是否保存令牌</param>
    /// <returns>返回用户请求</returns>
    public async Task<DataResponse<UserData>?> LoginByTokenAsync(string userToken, bool saveToken = true)
    {
        var forecast = await _client.GetFromJsonAsync("userinfo?token=" + userToken, Default.DataResponseUserData);
        if (forecast!.State)
        {
            var token = forecast.Data?.UserToken;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token!);
            if (saveToken)
                SaveToken(token!);
        }

        return forecast;
    }


    /// <summary>
    /// 自动登录
    /// </summary>
    /// <returns>返回用户请求</returns>
    /// <exception cref="NullReferenceException">读取文件失败</exception>
    public async Task<DataResponse<UserData>?> AutoLoginAsync()
    {
        if (File.Exists(TokenFilePath))
        {
            await using var stream = File.OpenRead(TokenFilePath);
            using var doc = await JsonDocument.ParseAsync(stream);

            if (doc.RootElement.TryGetProperty("usertoken", out var tokenElement))
            {
                var userToken = tokenElement.GetString()!;

                if (!string.IsNullOrWhiteSpace(userToken))
                    return await LoginByTokenAsync(userToken, false);
            }

            throw new NullReferenceException("Token doesn't exists.");
        }

        throw new NullReferenceException("File not found.");
    }

    /// <summary>
    /// 刷新用户信息
    /// </summary>
    /// <returns>返回用户请求</returns>
    public async Task<DataResponse<UserData>?> RefreshAsync() =>
        await Get<DataResponse<UserData>>("userinfo", Default.DataResponseUserData);

    /// <summary>
    /// 获取隧道请求
    /// </summary>
    /// <returns>返回隧道请求</returns>
    public async Task<DataResponse<IReadOnlyList<TunnelData>>?> GetTunnelResponseAsync() =>
        await Get<DataResponse<IReadOnlyList<TunnelData>>>("tunnel", Default.DataResponseIReadOnlyListTunnelData);

    /// <summary>
    /// 获取节点请求
    /// </summary>
    /// <returns>返回节点请求</returns>
    public async Task<DataResponse<IReadOnlyList<NodeData>>?> GetNodeResponseAsync() =>
        await Get<DataResponse<IReadOnlyList<NodeData>>>("node", Default.DataResponseIReadOnlyListNodeData);

    /// <summary>
    /// 获取节点详情请求
    /// </summary>
    /// <param name="node">节点数据类</param>
    /// <returns>返回节点请求</returns>
    public async Task<DataResponse<NodeInfo>?> GetNodeInfoResponseAsync(NodeData node) =>
        await Get<DataResponse<NodeInfo>>($"nodeinfo?node={node.Name}", Default.DataResponseNodeInfo);

    /// <summary>
    /// 重置用户Token
    /// </summary>
    /// <returns>请求结果</returns>
    public async Task<BaseResponse?> ResetTokenAsync() =>
        await Get<BaseResponse>("retoken", Default.BaseResponse);

    /// <summary>
    /// 更新用户QQ号
    /// </summary>
    /// <returns>请求结果</returns>
    /// <param name="newQQ">新QQ号</param>
    public async Task<BaseResponse?> UpdateQQAsync(string newQQ) =>
        await Get<BaseResponse>($"update_qq?new_qq={newQQ}", Default.BaseResponse);

    /// <summary>
    /// 更新用户名
    /// </summary>
    /// <param name="newName">新名字</param>
    /// <returns>请求结果</returns>
    public async Task<BaseResponse?> UpdateNameAsync(string newName) =>
        await Get<BaseResponse>($"update_username?new_username={newName}", Default.BaseResponse);


    /// <summary>
    /// 创建隧道请求
    /// </summary>
    /// <param name="request">请求数据</param>
    /// <returns>请求结果</returns>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<DataResponse<TunnelData>?> CreateTunnelAsync(CreateTunnelRequest request) =>
        await Post<CreateTunnelRequest, DataResponse<TunnelData>>
            ("create_tunnel", request, Default.CreateTunnelRequest, Default.DataResponseTunnelData);

    /// <summary>
    /// 更新隧道请求
    /// </summary>
    /// <param name="request">请求数据</param>
    /// <returns>请求结果</returns>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<DataResponse<TunnelData>?> UpdateTunnelAsync(UpdateTunnelRequest request) =>
        await Post<UpdateTunnelRequest, DataResponse<TunnelData>>
            ("update_tunnel", request, Default.UpdateTunnelRequest, Default.DataResponseTunnelData);

    private async Task<T?> Get<T>(string url, JsonTypeInfo<T> jsonTypeInfo)
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");

        return await _client.GetFromJsonAsync(url, jsonTypeInfo);
    }

    private async Task<TResponse?> Post<TRequest, TResponse>
    (
        string url,
        TRequest request,
        JsonTypeInfo<TRequest> requestJsonTypeInfo,
        JsonTypeInfo<TResponse> responseJsonTypeInfo
    )
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");

        using var response = await _client.PostAsync(url, JsonContent.Create(request, requestJsonTypeInfo));

        return await response.Content.ReadFromJsonAsync(responseJsonTypeInfo);
    }
}