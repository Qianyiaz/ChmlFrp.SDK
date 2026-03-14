using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;

namespace ChmlFrp.SDK.Service;

/// <summary>
/// 更新隧道请求服务
/// </summary>
public static class UpdateTunnelRequestService
{
    extension(UpdateTunnelRequest updateRequest)
    {
        /// <summary>
        /// 更新隧道
        /// </summary>
        /// <param name="client">ChmlFrp客户端</param>
        /// <returns>请求结果</returns>
        public async Task<BaseResponse?> UpdateAsync(ChmlFrpClient client) =>
            await client.UpdateTunnelAsync(updateRequest);
    }
}