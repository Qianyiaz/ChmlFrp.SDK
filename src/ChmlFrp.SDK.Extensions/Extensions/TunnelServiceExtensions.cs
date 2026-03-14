using System.Diagnostics;
using System.Text;
using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;
using ChmlFrp.SDK.Service;

namespace ChmlFrp.SDK.Extensions;

/// <summary>
/// 对隧道相关的操作
/// </summary>
public static class TunnelServiceExtensions
{
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
            if (!client.HasToken(out var token))
                throw new InvalidOperationException("未登录，无法启动隧道。");

            if (tunnel.IsRunning())
                throw new InvalidOperationException("隧道已在运行。");

            var frpProcess = StartFrpcProcess(token!, tunnel.Id.ToString(), options);
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
            if (!client.HasToken(out var token))
                throw new InvalidOperationException("未登录，无法启动隧道。");

            var tunnelList = tunnels.ToList();

            if (tunnelList.Count == 0)
                throw new ArgumentException("隧道集合不能为空。", nameof(tunnels));

            if (tunnelList.Any(t => t.IsRunning()))
                throw new ArgumentException("集合中包含已在运行的隧道。", nameof(tunnels));

            var ids = string.Join(",", tunnelList.Select(t => t.Id.ToString()));
            var frpProcess = StartFrpcProcess(token!, ids, options);

            frpProcess.Exited += (_, _) =>
            {
                foreach (var tunnel in tunnelList)
                    TunnelProcessExtensions.ProcessInfos.Remove(tunnel);
            };

            foreach (var tunnel in tunnelList)
                tunnel.SetFrpProcess(frpProcess);
        }

        /// <summary>
        /// 关闭隧道
        /// </summary>
        /// <param name="tunnel">隧道类</param>
        /// <returns>是否关闭成功</returns>
        public void StopTunnel(TunnelData tunnel)
        {
            if (!tunnel.IsRunning())
                throw new ArgumentException("隧道未运行。", nameof(tunnel));

            tunnel.GetFrpProcess()!.Kill(true);
        }

        /// <summary>
        /// 关闭隧道(多个)
        /// </summary>
        /// <param name="tunnels">隧道类列表</param>
        public void StopTunnel(IEnumerable<TunnelData> tunnels)
        {
            var tunnelList = tunnels.ToList();
            if (tunnelList.Count == 0)
                throw new ArgumentException("隧道集合不能为空。", nameof(tunnels));

            var uniqueProcesses = (from t in tunnelList where t.IsRunning() select t.GetFrpProcess()).ToList();
            foreach (var process in uniqueProcesses.OfType<Process>())
            {
                try
                {
                    process.Kill(true);
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// 重启隧道
        /// </summary>
        /// <param name="tunnel"></param>
        public void Restart(TunnelData tunnel)
        {
            client.StopTunnel(tunnel);
            client.StartTunnel(tunnel);
        }

        /// <summary>
        /// 重启多个隧道
        /// </summary>
        /// <param name="tunnels"></param>
        public void Restart(IEnumerable<TunnelData> tunnels)
        {
            var tunnelDatas = tunnels.ToList();
            client.StopTunnel(tunnelDatas);
            client.StartTunnel(tunnelDatas);
        }
    }

    private static Process StartFrpcProcess(string token, string id, TunnelStartOptions? options)
    {
        options ??= new TunnelStartOptions();
        var frpcFile = options.FrpcFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frpc");
        var command = options.CommandSuffix ?? "-u %token% -p %id%";
        var arguments = command.Replace("%token%", token).Replace("%id%", id);

        StreamWriter? logWriter = null;

        if (options.IsUseLogFile)
        {
            var logFile = options.LogFilePath ??
                          Path.Combine(Path.GetTempPath(), $"frpc_{DateTime.Now:yyyyMMddHHmmss}.log");

            var fileStream = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.Read);
            logWriter = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
        }

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

        var writer = logWriter;

        void OnOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            var line = args.Data!;
            writer?.WriteLine(line);
            options.Handler?.Invoke(line);
        }

        process.OutputDataReceived += OnOutputDataReceived;

        process.Exited += (_, _) =>
        {
            process.OutputDataReceived -= OnOutputDataReceived;
            logWriter?.Dispose();
            logWriter = null;
        };

        process.Start();
        process.BeginOutputReadLine();

        return process;
    }

    extension(TunnelData tunnel)
    {
        /// <summary>
        /// 启动隧道
        /// </summary>
        /// <param name="client"></param>
        public void Start(ChmlFrpClient client) => client.StartTunnel(tunnel);

        /// <summary>
        /// 停止隧道
        /// </summary>
        /// <param name="client"></param>
        public void Stop(ChmlFrpClient client) => client.StopTunnel(tunnel);

        /// <summary>
        /// 停止并删除隧道
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<BaseResponse?> DeleteWithStopAsync(ChmlFrpClient client)
        {
            tunnel.Stop(client);
            return await client.DeleteTunnelAsync(tunnel);
        }

        /// <summary>
        /// 重启隧道
        /// </summary>
        /// <param name="client"></param>
        public void Restart(ChmlFrpClient client) => client.Restart(tunnel);
    }

    extension(IEnumerable<TunnelData> tunnels)
    {
        /// <summary>
        /// 启动所有隧道
        /// </summary>
        /// <param name="client"></param>
        public void StartAll(ChmlFrpClient client) => client.StartTunnel(tunnels);

        /// <summary>
        /// 停止所有隧道
        /// </summary>
        /// <param name="client"></param>
        public void StopAll(ChmlFrpClient client) => client.StopTunnel(tunnels);

        /// <summary>
        /// 停止并删除所有隧道
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<BaseResponse>> DeleteWithStopAsync(ChmlFrpClient client)
        {
            var tunnelDatas = tunnels.ToList();
            tunnelDatas.StopAll(client);

            return (await Task.WhenAll(tunnelDatas.Select(client.DeleteTunnelAsync).ToList()))!;
        }

        /// <summary>
        /// 重启所有隧道
        /// </summary>
        /// <param name="client"></param>
        public void Restart(ChmlFrpClient client) => client.Restart(tunnels);
    }

    /// <summary>
    /// 隧道启动配置
    /// </summary>
    public sealed class TunnelStartOptions
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