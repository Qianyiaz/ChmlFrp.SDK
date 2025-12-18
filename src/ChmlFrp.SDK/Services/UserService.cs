using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ChmlFrp.SDK.Responses;

namespace ChmlFrp.SDK.Services;

/// <summary>
/// 用户操作
/// </summary>
public static class UserService
{
    private static readonly string TokenFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChmlFrp", "user.json");

    private static readonly HttpClient MainClient = new()
    {
        BaseAddress = new("https://cf-v2.uapis.cn/")
    };

    /// <summary>
    /// 用户操作相关的扩展方法
    /// </summary>
    /// <param name="user">用户类</param>
    extension(UserResponse user)
    {
        #region Json存储

        /// <summary>
        /// 删除本地保存的用户令牌
        /// </summary>
        public void LoginoutAsync()
        {
            if (File.Exists(TokenFilePath))
                File.Delete(TokenFilePath);
        }

        private static void SaveTokenAsync(string? userToken)
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

        #endregion

        #region 登录方法

        /// <summary>
        /// 使用用户名和密码登录获取用户信息
        /// </summary>
        /// <param name="username">用户名用于登录</param>
        /// <param name="password">密码</param>
        /// <param name="saveToken">是否保存令牌</param>
        /// <returns>返回用户请求</returns>
        public static async Task<UserResponse?> LoginAsync(string? username, string? password, bool saveToken = true)
        {
            try
            {
                var forecast = await MainClient.GetFromJsonAsync(
                    $"login?username={username}&password={password}",
                    SourceGeneration.Default.UserResponse
                );

                if (forecast!.State && saveToken)
                    SaveTokenAsync(forecast.Data!.UserToken);
                return forecast;
            }
            catch (Exception ex) when (ex is HttpRequestException)
            {
                return new UserResponse
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
        public static async Task<UserResponse?> LoginByTokenAsync(string userToken, bool saveToken = true)
        {
            try
            {
                var forecast = await MainClient.GetFromJsonAsync(
                    $"userinfo?token={userToken}",
                    SourceGeneration.Default.UserResponse
                );

                if (forecast!.State && saveToken)
                    SaveTokenAsync(forecast.Data!.UserToken);
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
        public static async Task<UserResponse?> AutoLoginAsync()
        {
            if (!File.Exists(TokenFilePath))
                return new()
                {
                    StateString = "fail",
                    Message = "File not found."
                };

            await using (var stream = File.OpenRead(TokenFilePath))
            {
                using (var doc = await JsonDocument.ParseAsync(stream))
                {
                    if (doc.RootElement.TryGetProperty("usertoken", out var tokenElement))
                    {
                        var userToken = tokenElement.GetString();

                        if (!string.IsNullOrWhiteSpace(userToken))
                            return await LoginByTokenAsync(userToken);
                    }
                }
            }

            try
            {
                File.Delete(TokenFilePath);
            }
            catch
            {
                // ignored
            }

            return new()
            {
                StateString = "fail",
                Message = "Invalid or empty token."
            };
        }

        #endregion

        /// <summary>
        /// 获取隧道请求
        /// </summary>
        /// <returns>返回隧道请求</returns>
        public async Task<TunnelResponse?> GetTunnelResponseAsync()
        {
            if (!user.State)
                return new()
                {
                    StateString = "fail",
                    Message = "You don't login."
                };

            try
            {
                return await MainClient.GetFromJsonAsync(
                    $"tunnel?token={user.Data!.UserToken}",
                    SourceGeneration.Default.TunnelResponse
                );
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
        public async Task<NodeResponse?> GetNodeResponseAsync()
        {
            if (!user.State)
                return new()
                {
                    StateString = "fail",
                    Message = "You don't login."
                };

            try
            {
                return await MainClient.GetFromJsonAsync(
                    $"node?token={user.Data!.UserToken}",
                    SourceGeneration.Default.NodeResponse
                );
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
        public async Task<NodeInfoResponse?> GetNodeInfoResponseAsync(NodeData node)
        {
            if (!user.State)
                return new()
                {
                    StateString = "fail",
                    Message = "You don't login."
                };

            try
            {
                return await MainClient.GetFromJsonAsync(
                    $"nodeinfo?token={user.Data!.UserToken}&node={node.Name}",
                    SourceGeneration.Default.NodeInfoResponse
                );
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
            try
            {
                return await MainClient.GetFromJsonAsync(
                    $"retoken?token={user.Data!.UserToken}",
                    SourceGeneration.Default.BaseResponse
                );
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
        public async Task<BaseResponse?> UpdateQqAsync(string newQq)
        {
            try
            {
                return await MainClient.GetFromJsonAsync(
                    $"update_qq?token={user.Data!.UserToken}&new_qq={newQq}",
                    SourceGeneration.Default.BaseResponse
                );
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
            try
            {
                return await MainClient.GetFromJsonAsync(
                    $"update_username?token={user.Data!.UserToken}&new_username={newName}",
                    SourceGeneration.Default.BaseResponse
                );
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
    }
}