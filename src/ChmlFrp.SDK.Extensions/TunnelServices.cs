using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ChmlFrp.SDK.Results;

namespace ChmlFrp.SDK.Extensions;

public static class TunnelDataExtensions
{
    private static readonly ConditionalWeakTable<TunnelData, ProcessInfo> ProcessInfos = new();

    private class ProcessInfo
    {
        public Process FrpProcess { get; init; }
    }

    public static void SetFrpProcess(this TunnelData tunnel, Process process)
    {
        ProcessInfos.AddOrUpdate(tunnel, new() { FrpProcess = process });
    }

    public static Process GetFrpProcess(this TunnelData tunnel)
    {
        return ProcessInfos.TryGetValue(tunnel, out var info) ? info?.FrpProcess : null;
    }

    public static bool IsRunning(this TunnelData tunnel)
    {
        var process = tunnel.GetFrpProcess();
        return process is { HasExited: false };
    }
}

/// <summary>
///     对隧道相关的操作.
/// </summary>
public static class TunnelServices
{
    public enum TunnelStatus
    {
        Failed,
        Succeed,
        AlreadyRunning
    }

    /// <summary>
    ///     你需要把FRPC文件放在当前执行目录才能启动
    /// </summary>
    public static void StartTunnel
    (
        this UserResult user,
        TunnelData tunnel,
        Action<TunnelStatus> onStatus = null,
        string logFilePath = null
    )
    {
        if (tunnel.IsRunning())
        {
            onStatus?.Invoke(TunnelStatus.AlreadyRunning);
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = "+x frpc",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            })!.WaitForExit();
        }

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
                Arguments = $"-u {user.Data.UserToken} -p {tunnel.Id}",
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
                onStatus?.Invoke(TunnelStatus.Failed);
            }
            else if (!succeed && line.Contains("启动成功"))
            {
                succeed = true;
                onStatus?.Invoke(TunnelStatus.Succeed);
            }
        };

        frpProcess.Start();
        frpProcess.BeginOutputReadLine();
        tunnel.SetFrpProcess(frpProcess);
    }

    /// <summary>
    ///     你需要把FRPC文件放在当前执行目录才能启动
    /// </summary>
    public static void StartTunnels
    (
        this UserResult user,
        List<TunnelData> tunnels,
        Action<TunnelStatus> onStatus = null,
        string logFilePath = null
    )
    {
        if (tunnels.Any(tunnel => tunnel.IsRunning()))
        {
            onStatus?.Invoke(TunnelStatus.AlreadyRunning);
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
                Arguments = $"-u {user.Data.UserToken} -p {ids}",
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
                onStatus?.Invoke(TunnelStatus.Failed);
            }
            else if (!succeed && line.Contains("启动成功"))
            {
                succeed = true;
                onStatus?.Invoke(TunnelStatus.Succeed);
            }
        };

        frpProcess.Start();
        frpProcess.BeginOutputReadLine();
        tunnels.ForEach(tunnel => tunnel.SetFrpProcess(frpProcess));
    }

    public static bool StopTunnel
    (
        this UserResult _,
        TunnelData tunnel
    )
    {
        if (!tunnel.IsRunning())
            return false;
        tunnel.GetFrpProcess()?.Kill(true);
        return true;
    }

    public static bool StopTunnels
    (
        this UserResult user,
        List<TunnelData> tunnels
    )
    {
        tunnels.ForEach(tunnel => StopTunnel(user, tunnel));
        return true;
    }
}