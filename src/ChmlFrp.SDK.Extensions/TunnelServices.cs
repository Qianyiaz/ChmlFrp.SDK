using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ChmlFrp.SDK.Results;

namespace ChmlFrp.SDK.Extensions;

/// <summary>
/// 对隧道相关的操作
/// </summary>
public static class TunnelServices
{
    /// <summary>
    /// 启动隧道
    /// </summary>
    /// <param name="user">用户类</param>
    extension(UserResult user)
    {
        /// <summary>
        /// 启动隧道
        /// </summary>
        /// <param name="tunnel">隧道类</param>
        /// <param name="options">启动配置</param>
        /// <exception cref="ArgumentNullException">设置frpc路径错误</exception>
        public void StartTunnel
        (
            TunnelData tunnel,
            TunnelStartOptions? options = null
        )
        {
            if (tunnel.IsRunning())
                return;
            
            tunnel.SetFrpProcess(StartFrpcProcess(user, tunnel.Id.ToString(), options));
        }

        /// <summary>
        /// 启动隧道(多个)
        /// </summary>
        /// <param name="tunnels">隧道类列表</param>
        /// <param name="options">启动配置</param>
        public void StartTunnels
        (
            List<TunnelData> tunnels,
            TunnelStartOptions? options = null
        )
        {
            if (tunnels.Any(tunnel => tunnel.IsRunning()))
                return;
            
            var ids = string.Join(",", tunnels.Select(t => t.Id.ToString()));
            var frpcProcess = StartFrpcProcess(user, ids, options);
            tunnels.ForEach(tunnel => tunnel.SetFrpProcess(frpcProcess));
        }

        /// <summary>
        /// 启动隧道进程
        /// </summary>
        /// <param name="id">隧道id</param>
        /// <param name="options">启动配置</param>
        /// <returns>启动隧道进程</returns>
        public Process StartFrpcProcess
        (
            string id,
            TunnelStartOptions? options
        )
        {
            var frpcfile = options?.FrpcFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frpc");
            var logfile = options?.LogFilePath ?? Path.GetTempFileName();
            
            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(frpcfile,
                    File.GetUnixFileMode(frpcfile)
                    | UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute);

            var frpProcess = new Process
            {
                StartInfo =
                {
                    FileName = frpcfile,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    Arguments = $"-u {user.Data?.UserToken} -p {id}{options?.CommandSuffix}"
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
        public void StopTunnel
        (
            TunnelData tunnel
        )
        {
            if (!tunnel.IsRunning())
                return;
            tunnel.GetFrpProcess()?.Kill(true);
        }

        /// <summary>
        /// 关闭隧道(多个)
        /// </summary>
        /// <param name="tunnels">隧道类列表</param>
        /// <returns>是否关闭成功</returns>
        public void StopTunnels(List<TunnelData> tunnels)
        {
            foreach (var tunnel in tunnels.Where(tunnel => tunnel.IsRunning()))
                StopTunnel(user, tunnel);
        }
    }

    /// <summary>
    /// 隧道启动配置
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
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