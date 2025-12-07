using System.Net.Http.Json;
using System.Text.Json;
using ChmlFrp.SDK.Results;

namespace ChmlFrp.SDK.Extensions;

/// <summary>
/// 对用户相关的操作
/// </summary>
public static class UserServices
{
    /// <summary>
    /// 用户操作相关的扩展方法
    /// </summary>
    /// <param name="user">用户类</param>
    extension(UserResult user)
    {
        /// <summary>
        /// 重置用户Token
        /// </summary>
        /// <returns>请求结果</returns>
        public async Task<BaseResult?> ResetTokenAsync()
        {
            try
            {
                return await BaseResult.MainClient.GetFromJsonAsync(
                    $"retoken?token={user.Data!.UserToken}",
                    SourceGeneration.Default.BaseResult
                );
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException)
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
        public async Task<BaseResult?> UpdateQqAsync(string newQq)
        {
            try
            {
                return await BaseResult.MainClient.GetFromJsonAsync(
                    $"update_qq?token={user.Data!.UserToken}&new_qq={newQq}",
                    SourceGeneration.Default.BaseResult
                );
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException)
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
        public async Task<BaseResult?> UpdateNameAsync(string newName)
        {
            try
            {
                return await BaseResult.MainClient.GetFromJsonAsync(
                    $"update_username?token={user.Data!.UserToken}&new_username={newName}",
                    SourceGeneration.Default.BaseResult
                );
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException)
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