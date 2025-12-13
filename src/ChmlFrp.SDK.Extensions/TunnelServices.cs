using System.Diagnostics;
using System.Text;
using ChmlFrp.SDK.Models;
using ChmlFrp.SDK.Responses;

namespace ChmlFrp.SDK.Extensions;

/// <summary>
/// 对隧道相关的操作
/// </summary>
public static class TunnelServices
{
    /// <summary>
    /// 隧道操作相关的扩展方法
    /// </summary>
    /// <param name="user">用户类</param>
    extension(UserResponse user)
    {
        /// <summary>
        /// 启动隧道
        /// </summary>
        /// <param name="tunnel">隧道类</param>
        /// <param name="options">启动配置</param>
        /// <exception cref="ArgumentNullException">设置frpc路径错误</exception>
        public void StartTunnel(TunnelData tunnel, TunnelStartOptions? options = null)
        {
            if (tunnel.IsRunning()) return;
            var frpProcess = StartFrpcProcess(user, tunnel.Id.ToString(), options);
            frpProcess.Exited += (_, _) => TunnelProcess.ProcessInfos.Remove(tunnel);
            tunnel.SetFrpProcess(frpProcess);
        }

        /// <summary>
        /// 启动隧道(多个)
        /// </summary>
        /// <param name="tunnels">隧道类列表</param>
        /// <param name="options">启动配置</param>
        public void StartTunnel(IEnumerable<TunnelData> tunnels, TunnelStartOptions? options = null)
        {
            var tunnelDatas = tunnels.ToList();
            if (tunnelDatas.Any(tunnel => tunnel.IsRunning()))
                return;

            var ids = string.Join(",", tunnelDatas.Select(t => t.Id.ToString()));
            var frpProcess = StartFrpcProcess(user, ids, options);
            
            frpProcess.Exited += (_, _) =>
            {
                foreach (var tunnel in tunnelDatas)
                    TunnelProcess.ProcessInfos.Remove(tunnel);
            };
            
            foreach (var tunnel in tunnelDatas)
                tunnel.SetFrpProcess(frpProcess);
        }
        
        private Process StartFrpcProcess(string id, TunnelStartOptions? options)
        {
            var frpcfile = options?.FrpcFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frpc");
            var logfile = options?.LogFilePath ?? Path.GetTempFileName();

            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(frpcfile,
                    UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute);

            var frpProcess = new Process
            {
                StartInfo =
                {
                    FileName = frpcfile,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    Arguments = $"-u {user.Data!.UserToken} -p {id}{options?.CommandSuffix}"
                }
            };

            File.WriteAllText(logfile, string.Empty);
            frpProcess.OutputDataReceived += (_, args) =>
            {
                var line = args.Data;
                if (string.IsNullOrWhiteSpace(line))
                    return;
                File.AppendAllText(logfile, line + Environment.NewLine);
                options?.Handler?.Invoke(line);
            };
            
            frpProcess.Start();
            frpProcess.BeginOutputReadLine();
            return frpProcess;
        }

        /// <summary>
        /// 关闭隧道
        /// </summary>
        /// <param name="tunnel">隧道类</param>
        /// <returns>是否关闭成功</returns>
        public void StopTunnel(TunnelData tunnel)
        {
            if (!tunnel.IsRunning()) return;
            tunnel.GetFrpProcess()!.Kill(true);
        }

        /// <summary>
        /// 关闭隧道(多个)
        /// </summary>
        /// <param name="tunnels">隧道类列表</param>
        /// <returns>是否关闭成功</returns>
        public void StopTunnel(IEnumerable<TunnelData> tunnels)
        {
            foreach (var tunnel in tunnels.Where(tunnel => tunnel.IsRunning()))
                StopTunnel(user, tunnel);
        }
    }

    /// <summary>
    /// 隧道启动配置
    /// </summary>
    public class TunnelStartOptions
    {
        /// <summary>
        /// 日志文件
        /// </summary>
        public string? LogFilePath { get; init; }

        /// <summary>
        /// frpc文件
        /// </summary>
        public string? FrpcFilePath { get; init; }

        /// <summary>
        /// 命令后缀
        /// </summary>
        public string CommandSuffix { get; init; } = string.Empty;

        /// <summary>
        /// 输出处理程序
        /// </summary>
        public Action<string>? Handler { get; init; }
    }
}