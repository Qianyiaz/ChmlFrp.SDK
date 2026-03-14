using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;

namespace ChmlFrp.SDK.Service;

/// <summary>
/// 创建隧道请求服务
/// </summary>
public static class CreateTunnelRequestService
{
    extension(CreateTunnelRequest createRequest)
    {
        /// <summary>
        /// 创建隧道
        /// </summary>
        /// <param name="client">ChmlFrp客户端</param>
        /// <returns>请求结果</returns>
        public async Task<BaseResponse?> CreateAsync(ChmlFrpClient client) =>
            await client.CreateTunnelAsync(createRequest);
    }
}