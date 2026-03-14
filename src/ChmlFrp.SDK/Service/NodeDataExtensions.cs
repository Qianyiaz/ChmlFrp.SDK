using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;

namespace ChmlFrp.SDK.Service;

/// <summary>
/// 节点数据扩展
/// </summary>
public static class NodeDataExtensions
{
    extension(NodeData node)
    {
        /// <summary>
        /// 获取节点详情
        /// </summary>
        /// <param name="client">ChmlFrp客户端</param>
        /// <returns>节点详情响应</returns>
        public async Task<DataResponse<NodeInfo>?> GetInfoAsync(ChmlFrpClient client) =>
            await client.GetNodeInfoResponseAsync(node);
    }
}