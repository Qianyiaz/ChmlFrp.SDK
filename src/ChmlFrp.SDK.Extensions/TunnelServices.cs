using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ChmlFrp.SDK.Results;

namespace ChmlFrp.SDK.Extensions;

/// <summary>
///     对隧道相关的操作
/// </summary>
public static class TunnelServices
{
    /// <summary>
    ///     启动隧道
    /// </summary>
    /// <param name="user">用户类</param>
    extension(UserResult user)
    {
        /// <summary>
        ///     启动隧道
        /// </summary>
        /// <param name="tunnel">隧道类</param>
        /// <param name="onStatus">隧道状态事件</param>
        /// <param name="options">启动配置</param>
        /// <exception cref="ArgumentNullException">设置frpc路径错误</exception>
        public void StartTunnel
        (
            TunnelData tunnel,
            Action<TunnelStatus>? onStatus = null,
            TunnelStartOptions? options = null
        )
        {
            if (tunnel.IsRunning())
            {
                onStatus?.Invoke(new()
                {
                    IsSuccess = false,
                    Message = "Tunnel is already running."
                });
                return;
            }

            options ??= new();
            options.FrpcFilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frpc");
            options.LogFilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log{tunnel.Id}.text");

            tunnel.SetFrpProcess(StartProcess(user, tunnel.Id.ToString(), options, onStatus));
        }

        /// <summary>
        ///     启动隧道(多个)
        /// </summary>
        /// <param name="tunnels">隧道类列表</param>
        /// <param name="onStatus">隧道状态事件</param>
        /// <param name="options">启动配置</param>
        /// <exception cref="ArgumentNullException">设置frpc路径错误</exception>
        public void StartTunnels
        (
            List<TunnelData> tunnels,
            Action<TunnelStatus>? onStatus = null,
            TunnelStartOptions? options = null
        )
        {
            if (tunnels.Any(tunnel => tunnel.IsRunning()))
            {
                onStatus?.Invoke(new()
                {
                    IsSuccess = false,
                    Message = "One or more tunnels are already running."
                });
                return;
            }

            options ??= new();
            options.FrpcFilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frpc");
            var ids = string.Join(",", tunnels.Select(t => t.Id.ToString()).ToArray());
            options.LogFilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log{ids}.text");

            tunnels.ForEach(tunnel => tunnel.SetFrpProcess(StartProcess(user, ids, options, onStatus)));
        }

        /// <summary>
        ///     启动隧道进程
        /// </summary>
        /// <param name="id">隧道id</param>
        /// <param name="options">启动配置</param>
        /// <param name="onStatus">隧道状态事件</param>
        /// <returns>启动隧道进程</returns>
        public Process StartProcess
        (
            string id,
            TunnelStartOptions options,
            Action<TunnelStatus>? onStatus = null
        )
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(new ProcessStartInfo
                {
                    FileName = "chmod",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = $"+x \"{Path.GetFileName(options.FrpcFilePath)}\"",
                    WorkingDirectory = Path.GetDirectoryName(options.FrpcFilePath)
                })!.WaitForExit();

            var frpProcess = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    FileName = Path.GetFileName(options.FrpcFilePath),
                    WorkingDirectory = Path.GetDirectoryName(options.FrpcFilePath),
                    Arguments = $"-u {user.Data!.UserToken} -p {id}{options.Arguments}"
                }
            };

            var fail = false;
            var succeed = false;
            File.WriteAllText(options.LogFilePath!, string.Empty);

            frpProcess.OutputDataReceived += (_, args) =>
            {
                var line = args.Data;
                if (string.IsNullOrWhiteSpace(line))
                    return;
                File.AppendAllText(options.LogFilePath!, line + Environment.NewLine);

                if (!fail && !line.Contains("[I]"))
                {
                    fail = true;
                    frpProcess.Kill(true);
                    onStatus?.Invoke(new()
                    {
                        IsSuccess = false,
                        Message = "Tunnel failed to start."
                    });
                }
                else if (!succeed && line.Contains("启动成功"))
                {
                    succeed = true;
                    onStatus?.Invoke(new()
                    {
                        IsSuccess = true,
                        Message = "Tunnel started successfully."
                    });
                }
            };

            frpProcess.Start();
            frpProcess.BeginOutputReadLine();
            return frpProcess;
        }

        /// <summary>
        ///     关闭隧道
        /// </summary>
        /// <param name="tunnel">隧道类</param>
        /// <returns>是否关闭成功</returns>
        public bool StopTunnel
        (
            TunnelData tunnel
        )
        {
            if (!tunnel.IsRunning())
                return false;
            tunnel.GetFrpProcess()?.Kill(true);
            return true;
        }

        /// <summary>
        ///     关闭隧道(多个)
        /// </summary>
        /// <param name="tunnels">隧道类列表</param>
        /// <returns>是否关闭成功</returns>
        public bool StopTunnels
        (
            List<TunnelData> tunnels
        )
        {
            tunnels.ForEach(tunnel => StopTunnel(user, tunnel));
            return true;
        }
    }

    /// <summary>
    ///     隧道启动配置
    /// </summary>
    public class TunnelStartOptions
    {
        /// <summary>
        ///     日志文件
        /// </summary>
        public string? LogFilePath { get; set; }

        /// <summary>
        ///     frpc文件
        /// </summary>
        public string? FrpcFilePath { get; set; }

        /// <summary>
        ///     命令后缀
        /// </summary>
        public string Arguments { get; init; } = string.Empty;
    }

    /// <summary>
    ///     隧道状态
    /// </summary>
    public class TunnelStatus
    {
        /// <summary>
        ///     运行成功
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        ///     状态消息
        /// </summary>
        public string Message { get; init; } = string.Empty;
    }
}