using System.Net.Http.Json;
using System.Text.Json;
using ChmlFrp.SDK.Content;
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
    
    private string? _token;

    /// <summary>
    /// 判断是否登录
    /// </summary>
    /// <param name="tokenEscaped">用户token</param>
    /// <returns>是否登录</returns>
    public bool HasToken(out string tokenEscaped)
    {
        tokenEscaped = Uri.EscapeDataString(_token ?? string.Empty);
        return !string.IsNullOrWhiteSpace(_token);
    }
    
    private readonly HttpClient _client;
    
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="client">http客户端</param>
    public ChmlFrpClient(HttpClient? client = null)
    {
        _client = client ?? new HttpClient { BaseAddress = new Uri("https://cf-v2.uapis.cn") };
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
        try
        {
            var forecast = await _client.GetFromJsonAsync($"login?username={username}&password={password}", Default.DataResponseUserData);
            if (!forecast!.State) return forecast;
            
            _token = forecast.Data?.UserToken;
            if (saveToken)
                SaveToken(_token);
            return forecast;
        }
        catch (Exception ex) when (ex is HttpRequestException)
        {
            return new()
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    ///  使用用户令牌获取用户信息
    /// </summary>
    /// <param name="userToken">用户令牌用于登录</param>
    /// <param name="saveToken">是否保存令牌</param>
    /// <returns>返回用户请求</returns>
    public async Task<DataResponse<UserData>?> LoginByTokenAsync(string userToken, bool saveToken = true)
    {
        try
        {
            var forecast = await _client.GetFromJsonAsync("userinfo?token=" + userToken, Default.DataResponseUserData);
            if (!forecast!.State) return forecast;
            
            _token = forecast.Data?.UserToken;
            if (saveToken)
                SaveToken(_token);
            return forecast;
        }
        catch (Exception ex) when (ex is HttpRequestException)
        {
            return new()
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// 自动登录
    /// </summary>
    /// <returns>返回用户请求</returns>
    public async Task<DataResponse<UserData>?> AutoLoginAsync()
    {
        if (!File.Exists(TokenFilePath))
            return new()
            {
                StateString = "fail",
                Message = "File not found."
            };

        using var stream = File.OpenRead(TokenFilePath);
        using var doc = await JsonDocument.ParseAsync(stream);
        
        if (doc.RootElement.TryGetProperty("usertoken", out var tokenElement))
        {
            var userToken = tokenElement.GetString()!;

            if (!string.IsNullOrWhiteSpace(userToken))
                return await LoginByTokenAsync(userToken,false);
        }

        return new()
        {
            StateString = "fail",
            Message = "Invalid or empty token."
        };
    }
    
    /// <summary>
    /// 获取隧道请求
    /// </summary>
    /// <returns>返回隧道请求</returns>
    public async Task<DataResponse<IReadOnlyList<TunnelData>>?> GetTunnelResponseAsync()
    {
        if (!HasToken(out var token))
            throw new NullReferenceException("Not logged in (token missing).");
        
        try
        {
            return await _client.GetFromJsonAsync("tunnel?token=" + token, Default.DataResponseIReadOnlyListTunnelData);
        }
        catch (Exception ex) when (ex is HttpRequestException)
        {
            return new()
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// 获取节点请求
    /// </summary>
    /// <returns>返回节点请求</returns>
    public async Task<DataResponse<IReadOnlyList<NodeData>>?> GetNodeResponseAsync()
    {
        if (!HasToken(out var token))
            throw new NullReferenceException("Not logged in (token missing).");
        
        try
        {
            return await _client.GetFromJsonAsync("node?token=" + token, Default.DataResponseIReadOnlyListNodeData);
        }
        catch (Exception ex) when (ex is HttpRequestException)
        {
            return new()
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// 获取节点详情请求
    /// </summary>
    /// <param name="node">节点数据类</param>
    /// <returns>返回节点请求</returns>
    public async Task<DataResponse<NodeInfo>?> GetNodeInfoResponseAsync(NodeData node)
    {
        if (!HasToken(out var token))
            throw new NullReferenceException("Not logged in (token missing).");
        
        try
        {
            return await _client.GetFromJsonAsync($"nodeinfo?token={token}&node={node.Name}", Default.DataResponseNodeInfo);
        }
        catch (Exception ex) when (ex is HttpRequestException)
        {
            return new()
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// 重置用户Token
    /// </summary>
    /// <returns>请求结果</returns>
    public async Task<BaseResponse?> ResetTokenAsync()
    {
        if (!HasToken(out var token))
            throw new NullReferenceException("Not logged in (token missing).");
        
        try
        {
            return await _client.GetFromJsonAsync("retoken?token=" + token, Default.BaseResponse);
        }
        catch (Exception ex) when (ex is HttpRequestException)
        {
            return new()
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// 更新用户QQ号
    /// </summary>
    /// <returns>请求结果</returns>
    public async Task<BaseResponse?> UpdateQQAsync(string newQQ)
    {
        if (!HasToken(out var token))
            throw new NullReferenceException("Not logged in (token missing).");
        
        try
        {
            return await _client.GetFromJsonAsync($"update_qq?token={token}&new_qq={newQQ}", Default.BaseResponse);
        }
        catch (Exception ex) when (ex is HttpRequestException)
        {
            return new()
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// 更新用户名
    /// </summary>
    /// <returns>请求结果</returns>
    public async Task<BaseResponse?> UpdateNameAsync(string newName)
    {
        if (!HasToken(out var token))
            throw new NullReferenceException("Not logged in (token missing).");
        
        try
        {
            return await _client.GetFromJsonAsync($"update_username?token={token}&new_username={newName}", Default.BaseResponse);
        }
        catch (Exception ex) when (ex is HttpRequestException)
        {
            return new()
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<DataResponse<TunnelData>?> CreateTunnelAsync(CreateTunnelRequest request)
    {
        if (!HasToken(out var token))
            throw new NullReferenceException("Not logged in (token missing).");

        try
        {
            using var content = JsonContent.Create(request, Default.CreateTunnelRequest);
            var response = await _client.PostAsync($"create_tunnel?token={token}", content);

            if (!response.IsSuccessStatusCode)
            {
                return new DataResponse<TunnelData>
                {
                    StateString = "fail",
                    Message = await response.Content.ReadAsStringAsync()
                };
            }

            var dataResponse = await response.Content.ReadFromJsonAsync<DataResponse<TunnelData>>(Default.DataResponseTunnelData);
            return dataResponse;
        }
        catch (HttpRequestException ex)
        {
            return new DataResponse<TunnelData>
            {
                StateString = "fail",
                Message = ex.Message
            };
        }
    }
}