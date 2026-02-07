using System.Diagnostics;
using System.Text;
using ChmlFrp.SDK.Models;
using ChmlFrp.SDK.Service;

namespace ChmlFrp.SDK.Extensions;

/// <summary>
/// 对隧道相关的操作
/// </summary>
public static class TunnelServiceExtensions
{
    /// <summary>
    /// 隧道操作相关的扩展方法
    /// </summary>
    /// <param name="client">客户端</param>
    extension(ChmlFrpClient client)
    {
        /// <summary>
        /// 启动隧道
        /// </summary>
        /// <param name="tunnel">隧道类</param>
        /// <param name="options">启动配置</param>
        /// <exception cref="ArgumentNullException">设置frpc路径错误</exception>
        public void StartTunnel(TunnelData tunnel, TunnelStartOptions? options = null)
        {
            if (tunnel.IsRunning())
                throw new ArgumentNullException(nameof(tunnel), "Tunnel is running.");

            var frpProcess = client.StartFrpcProcess(tunnel.Id.ToString()!, options);
            frpProcess.Exited += (_, _) => TunnelProcessExtensions.ProcessInfos.Remove(tunnel);
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

            if (tunnelDatas.Count == 0 || tunnelDatas.Any(tunnel => tunnel.IsRunning()))
                throw new ArgumentNullException(nameof(tunnels), "No tunnel or tunnel is running.");

            var ids = string.Join(",", tunnelDatas.Select(t => t.Id.ToString()));
            var frpProcess = client.StartFrpcProcess(ids, options);

            frpProcess.Exited += (_, _) =>
            {
                foreach (var tunnel in tunnelDatas)
                    TunnelProcessExtensions.ProcessInfos.Remove(tunnel);
            };

            foreach (var tunnel in tunnelDatas)
                tunnel.SetFrpProcess(frpProcess);
        }

        private Process StartFrpcProcess(string id, TunnelStartOptions? options)
        {
            if (!client.HasToken(out var token))
                throw new NullReferenceException("Not logged in (token missing).");

            var frpcfile = options?.FrpcFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frpc");
            var command = options?.CommandSuffix ?? "-u %token% -p %id%";

            string? logfile = null;
            var isUseLogFile = options?.IsUseLogFile ?? true;
            if (isUseLogFile)
                logfile = options?.LogFilePath ?? Path.GetTempFileName();

#if NET7_0_OR_GREATER
            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(frpcfile,
                    UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute);
#endif

            var frpProcess = new Process
            {
                StartInfo =
                {
                    FileName = frpcfile,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    Arguments = command.Replace("%token%", token).Replace("%id%", id)
                }
            };

            if (isUseLogFile)
                File.WriteAllText(logfile!, string.Empty);

            frpProcess.OutputDataReceived += (_, args) =>
            {
                var line = args.Data;
                if (string.IsNullOrWhiteSpace(line))
                    return;

                if (isUseLogFile)
                    File.AppendAllText(logfile!, line + Environment.NewLine);
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

#if NETCOREAPP3_0_OR_GREATER
            tunnel.GetFrpProcess()!.Kill(true);
#else
            tunnel.GetFrpProcess()!.Kill();
#endif
        }

        /// <summary>
        /// 关闭隧道(多个)
        /// </summary>
        /// <param name="tunnels">隧道类列表</param>
        /// <returns>是否关闭成功</returns>
        public void StopTunnel(IEnumerable<TunnelData> tunnels)
        {
            foreach (var tunnel in tunnels.Where(tunnel => tunnel.IsRunning()))
                client.StopTunnel(tunnel);
        }
    }

    /// <summary>
    /// 隧道启动配置
    /// </summary>
    public class TunnelStartOptions
    {
        /// <summary>
        /// 是否使用日志文件记录frpc输出
        /// </summary>
        public bool IsUseLogFile { get; set; } = true;

        /// <summary>
        /// 日志文件
        /// </summary>
        public string? LogFilePath { get; set; }

        /// <summary>
        /// frpc文件
        /// </summary>
        public string? FrpcFilePath { get; set; }

        /// <summary>
        /// 命令后缀
        /// </summary>
        public string? CommandSuffix { get; set; }

        /// <summary>
        /// 输出处理程序
        /// </summary>
        public Action<string>? Handler { get; set; }
    }
}