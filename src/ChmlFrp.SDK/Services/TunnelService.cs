using System.Net.Http.Json;
using ChmlFrp.SDK.Responses;

namespace ChmlFrp.SDK.Services;

/// <summary>
/// 隧道操作服务
/// </summary>
public static class TunnelService
{
    extension(UserResponse user)
    {
        /// <summary>
        /// 创建隧道
        /// </summary>
        /// <returns>返回隧道操作响应</returns>
        public async Task<CreateTunnelResponse?> CreateTunnelResponse(CreateTunnelRequest request)
        {
            try
            {
                switch (request.PortType)
                {
                    case "http" or "https" when string.IsNullOrEmpty(request.BandDomain):
                        return new()
                        {
                            StateString = "fail",
                            Message = "HTTP/HTTPS隧道必须提供绑定域名"
                        };
                    case "tcp" or "udp" when !request.RemotePort.HasValue:
                        return new()
                        {
                            StateString = "fail",
                            Message = "TCP/UDP隧道必须提供远程端口"
                        };
                }

                request.Token ??= user.Data!.UserToken;
                var response = await UserService.MainClient.PostAsJsonAsync("create_tunnel", request,
                    SourceGeneration.Default.CreateTunnelRequest);

                return await response.Content.ReadFromJsonAsync(SourceGeneration.Default.CreateTunnelResponse);
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

        /*
        /// <summary>
        /// 修改隧道
        /// </summary>
        /// <returns>返回隧道操作响应</returns>
        public async Task<CreateTunnelResponse?> UpdateTunnelAsync(CreateTunnelRequest request)
        {
            if (request.PortType is not ("tcp" or "udp"))
            {
                return new()
                {
                    StateString = "fail",
                    Message = "修改隧道仅支持TCP/UDP类型"
                };
            }

            request.Token ??= user.Data!.UserToken;
            try
            {
                var response = await UserService.MainClient.PostAsJsonAsync("update_tunnel", request,
                    SourceGeneration.Default.CreateTunnelRequest);

                return await response.Content.ReadFromJsonAsync(
                    SourceGeneration.Default.CreateTunnelResponse
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
        }*/
        // fuck you,chaoji🤬
    }
}