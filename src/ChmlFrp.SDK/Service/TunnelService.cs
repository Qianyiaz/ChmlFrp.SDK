using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;

namespace ChmlFrp.SDK.Service;

/// <summary>
/// 隧道服务
/// </summary>
public static class TunnelService
{
    extension(TunnelData tunnel)
    {
        /// <summary>
        /// 删除隧道
        /// </summary>
        /// <param name="client">ChmlFrp客户端</param>
        /// <returns>请求结果</returns>
        public async Task<BaseResponse?> DeleteAsync(ChmlFrpClient client) => await client.DeleteTunnelAsync(tunnel);
    }
}