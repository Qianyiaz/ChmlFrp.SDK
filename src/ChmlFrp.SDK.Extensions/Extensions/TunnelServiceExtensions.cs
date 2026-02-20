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
                throw new InvalidOperationException("隧道已在运行。");

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
            var tunnelList = tunnels.ToList();

            if (tunnelList.Count == 0)
                throw new ArgumentException("隧道集合不能为空。", nameof(tunnels));

            if (tunnelList.Any(t => t.IsRunning()))
                throw new ArgumentException("集合中包含已在运行的隧道。", nameof(tunnels));

            var ids = string.Join(",", tunnelList.Select(t => t.Id.ToString()));
            var frpProcess = client.StartFrpcProcess(ids, options);

            frpProcess.Exited += (_, _) =>
            {
                foreach (var tunnel in tunnelList)
                    TunnelProcessExtensions.ProcessInfos.Remove(tunnel);
            };

            foreach (var tunnel in tunnelList)
                tunnel.SetFrpProcess(frpProcess);
        }

        private Process StartFrpcProcess(string id, TunnelStartOptions? options)
        {
            if (!client.HasToken(out var token))
                throw new InvalidOperationException("未登录，无法启动隧道。");

            var frpcFile = options?.FrpcFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frpc");
            var command = options?.CommandSuffix ?? "-u %token% -p %id%";
            var arguments = command.Replace("%token%", token).Replace("%id%", id);

            var useLogFile = options?.IsUseLogFile ?? true;
            StreamWriter? logWriter = null;

            if (useLogFile)
            {
                var logFile = options?.LogFilePath ??
                              Path.Combine(Path.GetTempPath(), $"frpc_{DateTime.Now:yyyyMMddHHmmss}.log");

                var fileStream = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.Read);
                logWriter = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
            }

#if NET7_0_OR_GREATER
            if (!OperatingSystem.IsWindows())
            {
                try
                {
                    File.SetUnixFileMode(frpcFile,
                        UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute);
                }
                catch
                {
                    // ignored
                }
            }
#endif

            var process = new Process
            {
                StartInfo =
                {
                    FileName = frpcFile,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                }
            };

            void OnOutputDataReceived(object sender, DataReceivedEventArgs args)
            {
                if (string.IsNullOrWhiteSpace(args.Data)) return;

                var line = args.Data;
                // ReSharper disable once AccessToDisposedClosure
                logWriter?.WriteLine(line);
                options?.Handler?.Invoke(line);
            }

            process.OutputDataReceived += OnOutputDataReceived;

            process.Exited += (_, _) =>
            {
                process.OutputDataReceived -= OnOutputDataReceived;
                logWriter?.Dispose();
            };

            process.Start();
            process.BeginOutputReadLine();

            return process;
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
            {
#if NETCOREAPP3_0_OR_GREATER
                tunnel.GetFrpProcess()!.Kill(true);
#else
                tunnel.GetFrpProcess()!.Kill();
#endif
            }
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