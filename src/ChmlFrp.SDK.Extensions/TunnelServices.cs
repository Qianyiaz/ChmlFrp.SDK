using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ChmlFrp.SDK.Results;

namespace ChmlFrp.SDK.Extensions;

/// <summary>
/// 隧道进程相关的操作
/// </summary>
public static class TunnelProcess
{
    private static readonly ConditionalWeakTable<TunnelData, ProcessInfo> ProcessInfos = new();

    private class ProcessInfo
    {
        public Process? FrpProcess { get; init; }
    }

    /// <summary>
    /// 对隧道相关的操作
    /// </summary>
    /// <param name="tunnel">扩展隧道</param>
    extension(TunnelData tunnel)
    {
        /// <summary>
        /// 设置进程
        /// </summary>
        /// <param name="process">设置的进程</param>
        public void SetFrpProcess(Process process)
        {
            ProcessInfos.AddOrUpdate(tunnel, new() { FrpProcess = process });
        }

        /// <summary>
        /// 获取进程
        /// </summary>
        /// <returns>设置的进程</returns>
        public Process? GetFrpProcess()
        {
            return ProcessInfos.TryGetValue(tunnel, out var info) ? info.FrpProcess : null;
        }

        /// <summary>
        /// 获得隧道的进程是否在运行
        /// </summary>
        /// <returns>隧道的进程是否在运行</returns>
        public bool IsRunning()
        {
            var process = tunnel.GetFrpProcess();
            return process is { HasExited: false };
        }
    }
}

/// <summary>
///     对隧道相关的操作
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
        /// <param name="onStatus">隧道状态事件</param>
        /// <param name="logFilePath">log文件目录</param>
        public void StartTunnel
        (
            TunnelData tunnel,
            Action<TunnelStatus>? onStatus = null,
            string? logFilePath = null
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start(new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = "+x frpc",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                })!.WaitForExit();

            logFilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log{tunnel.Id}.text");
            var frpProcess = new Process
            {
                StartInfo =
                {
                    FileName = "frpc",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    Arguments = $"-u {user.Data!.UserToken} -p {tunnel.Id}",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            };

            var fail = false;
            var succeed = false;
            File.WriteAllText(logFilePath, string.Empty);
            frpProcess.OutputDataReceived += (_, args) =>
            {
                var line = args.Data;
                if (string.IsNullOrWhiteSpace(line))
                    return;
                File.AppendAllText(logFilePath, line + Environment.NewLine);

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
            tunnel.SetFrpProcess(frpProcess);
        }

        /// <summary>
        /// 启动隧道(多个)
        /// </summary>
        /// <param name="tunnels">隧道类列表</param>
        /// <param name="onStatus">隧道状态事件</param>
        /// <param name="logFilePath">log文件目录</param>
        public void StartTunnels
        (
            List<TunnelData> tunnels,
            Action<TunnelStatus>? onStatus = null,
            string? logFilePath = null
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start(new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = "+x frpc",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                })!.WaitForExit();

            var ids = string.Join(",", tunnels.Select(t => t.Id.ToString()).ToArray());
            logFilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log{ids}.text");

            var frpProcess = new Process
            {
                StartInfo =
                {
                    FileName = "frpc",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    Arguments = $"-u {user.Data!.UserToken} -p {ids}",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            };

            var fail = false;
            var succeed = false;
            File.WriteAllText(logFilePath, string.Empty);
            frpProcess.OutputDataReceived += (_, args) =>
            {
                var line = args.Data;
                if (string.IsNullOrWhiteSpace(line))
                    return;
                File.AppendAllText(logFilePath, line + Environment.NewLine);

                if (!fail && !line.Contains("[I]"))
                {
                    fail = true;
                    frpProcess.Kill(true);
                    onStatus?.Invoke(new()
                    {
                        IsSuccess = false,
                        Message = "One or more tunnels failed to start."
                    });
                }
                else if (!succeed && line.Contains("启动成功"))
                {
                    succeed = true;
                    onStatus?.Invoke(new()
                    {
                        IsSuccess = true,
                        Message = "All tunnels started successfully."
                    });
                }
            };

            frpProcess.Start();
            frpProcess.BeginOutputReadLine();
            tunnels.ForEach(tunnel => tunnel.SetFrpProcess(frpProcess));
        }

        /// <summary>
        /// 关闭隧道
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
        /// 关闭隧道(多个)
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
    /// 隧道状态
    /// </summary>
    public class TunnelStatus
    {
        /// <summary>
        /// 运行成功
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// 状态消息
        /// </summary>
        public string Message { get; init; } = string.Empty;
    }
}