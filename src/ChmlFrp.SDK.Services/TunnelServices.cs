using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ChmlFrp.SDK.Results;

namespace ChmlFrp.SDK.Services;

/// <summary>
///     对FRPC相关的操作.
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
        if (tunnel.IsRunning)
        {
            onStatus?.Invoke(TunnelStatus.AlreadyRunning);
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            new Process
            {
                StartInfo =
                {
                    FileName = "chmod",
                    CreateNoWindow = true,
                    Arguments = "+x frpc",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            }.Start();
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
                onStatus?.Invoke(TunnelStatus.Failed);
                frpProcess.Kill();
            }
            else if (!succeed && line.Contains("启动成功"))
            {
                succeed = true;
                onStatus?.Invoke(TunnelStatus.Succeed);
            }
        };

        frpProcess.Start();
        frpProcess.BeginOutputReadLine();
        tunnel.FrpProcess = frpProcess;
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
        if (tunnels.Any(tunnel => tunnel.IsRunning))
        {
            onStatus?.Invoke(TunnelStatus.AlreadyRunning);
            return;
        }

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
                onStatus?.Invoke(TunnelStatus.Failed);
                frpProcess.Kill();
            }
            else if (!succeed && line.Contains("启动成功"))
            {
                succeed = true;
                onStatus?.Invoke(TunnelStatus.Succeed);
            }
        };

        frpProcess.Start();
        frpProcess.BeginOutputReadLine();
        tunnels.ForEach(x => x.FrpProcess = frpProcess);
    }

    public static bool StopTunnel
    (
        this UserResult _,
        TunnelData tunnel
    )
    {
        if (!tunnel.IsRunning)
            return false;
        tunnel.FrpProcess?.Kill(true);
        return true;
    }

    public static bool StopTunnels
    (
        this UserResult user,
        List<TunnelData> tunnels
    )
    {
        foreach (var tunnel in tunnels)
            StopTunnel(user, tunnel);
        return true;
    }
}