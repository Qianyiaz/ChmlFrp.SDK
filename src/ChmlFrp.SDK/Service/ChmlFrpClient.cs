using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;
using static ChmlFrp.SDK.SourceGeneration;

namespace ChmlFrp.SDK.Service;

/// <summary>
/// ChmlFrp Http 请求客户端
/// </summary>
public class ChmlFrpClient
{
    private static readonly string TokenFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChmlFrp", "user.json");

    private readonly HttpClient _client;

    private string? _token;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="client">http客户端</param>
    public ChmlFrpClient(HttpClient? client = null) => _client = client ?? new HttpClient { BaseAddress = new Uri("https://cf-v2.uapis.cn") };

    /// <summary>
    /// 判断是否登录
    /// </summary>
    /// <param name="tokenEscaped">用户token</param>
    /// <returns>是否登录</returns>
    public bool HasToken(out string tokenEscaped)
    {
        tokenEscaped = _token ?? string.Empty;
        return !string.IsNullOrWhiteSpace(_token);
    }

    private void SaveToken(string? userToken)
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
        var forecast = await _client.GetFromJsonAsync($"login?username={username}&password={password}", Default.DataResponseUserData);
        if (forecast!.State)
        {
            _token = forecast.Data?.UserToken;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_token);
            if (saveToken)
                SaveToken(_token);
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
            _token = forecast.Data?.UserToken;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_token);
            if (saveToken)
                SaveToken(_token);
        }

        return forecast;
    }

    /// <summary>
    /// 自动登录
    /// </summary>
    /// <returns>返回用户请求</returns>
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
    /// 获取隧道请求
    /// </summary>
    /// <returns>返回隧道请求</returns>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<DataResponse<IReadOnlyList<TunnelData>>?> GetTunnelResponseAsync()
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");

        return await _client.GetFromJsonAsync("tunnel", Default.DataResponseIReadOnlyListTunnelData);
    }

    /// <summary>
    /// 获取节点请求
    /// </summary>
    /// <returns>返回节点请求</returns>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<DataResponse<IReadOnlyList<NodeData>>?> GetNodeResponseAsync()
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");
        
        return await _client.GetFromJsonAsync("node", Default.DataResponseIReadOnlyListNodeData);
    }

    /// <summary>
    /// 获取节点详情请求
    /// </summary>
    /// <param name="node">节点数据类</param>
    /// <returns>返回节点请求</returns>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<DataResponse<NodeInfo>?> GetNodeInfoResponseAsync(NodeData node)
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");

        return await _client.GetFromJsonAsync($"nodeinfo?node={node.Name}", Default.DataResponseNodeInfo);
    }

    /// <summary>
    /// 重置用户Token
    /// </summary>
    /// <returns>请求结果</returns>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<BaseResponse?> ResetTokenAsync()
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");
        
        return await _client.GetFromJsonAsync("retoken", Default.BaseResponse);
    }

    /// <summary>
    /// 更新用户QQ号
    /// </summary>
    /// <returns>请求结果</returns>
    /// <param name="newQQ">新QQ号</param>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<BaseResponse?> UpdateQQAsync(string newQQ)
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");

        return await _client.GetFromJsonAsync($"update_qq?new_qq={newQQ}", Default.BaseResponse);
    }

    /// <summary>
    /// 更新用户名
    /// </summary>
    /// <param name="newName">新名字</param>
    /// <returns>请求结果</returns>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<BaseResponse?> UpdateNameAsync(string newName)
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");
        
        return await _client.GetFromJsonAsync($"update_username?new_username={newName}", Default.BaseResponse);
    }


    /// <summary>
    /// 创建隧道请求
    /// </summary>
    /// <param name="request">请求数据</param>
    /// <returns>请求结果</returns>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<DataResponse<TunnelData>?> CreateTunnelAsync(CreateTunnelRequest request)
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");
        
        using var response = await _client.PostAsync("create_tunnel", JsonContent.Create(request, Default.CreateTunnelRequest));
        
        return await response.Content.ReadFromJsonAsync<DataResponse<TunnelData>>(Default.DataResponseTunnelData);
    }
    
    /// <summary>
    /// 更新隧道请求
    /// </summary>
    /// <param name="request">请求数据</param>
    /// <returns>请求结果</returns>
    /// <exception cref="NullReferenceException">未登录</exception>
    public async Task<DataResponse<TunnelData>?> UpdateTunnelAsync(UpdateTunnelRequest request)
    {
        if (!HasToken(out _))
            throw new NullReferenceException("Not logged in (token missing).");
        
        using var response = await _client.PostAsync("update_tunnel", JsonContent.Create(request, Default.CreateTunnelRequest));
        
        return await response.Content.ReadFromJsonAsync<DataResponse<TunnelData>>(Default.DataResponseTunnelData);
    }
}