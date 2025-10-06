using System.Diagnostics;
using System.Text;
using ChmlFrp.SDK.Results;

namespace ChmlFrp.SDK.Services;

/// <summary>
///     对FRPC相关的操作.
/// </summary>
public static class TunnelServices
{
    /// <summary>
    ///     你需要把FRPC文件放在当前执行目录才能启动
    /// </summary>
    public static void StartTunnel
    (
        this UserData user,
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
                Arguments = $"-u {user.UserToken} -p {tunnel.Id}",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
            }
        };

        var fail = true;
        File.WriteAllText(logFilePath, string.Empty);
        frpProcess.OutputDataReceived += (_, args) =>
        {
            var line = args.Data;
            if (string.IsNullOrWhiteSpace(line))
                return;
            File.AppendAllText(logFilePath, line + Environment.NewLine);

            if (fail && !line.Contains("[I]"))
            {
                fail = false;
                onStatus?.Invoke(TunnelStatus.Failed);
                frpProcess.Kill();
            }
            else if (line.Contains("启动成功"))
            {
                onStatus?.Invoke(TunnelStatus.Succeed);
            }
        };

        frpProcess.Start();
        frpProcess.BeginOutputReadLine();
        tunnel.FrpProcess = frpProcess;
    }

    public static bool StopTunnel
    (
        this UserData _,
        TunnelData tunnel
    )
    {
        if (!tunnel.IsRunning)
            return false;
        tunnel.FrpProcess.Close();
        if (!tunnel.FrpProcess.WaitForExit(100))
            tunnel.FrpProcess.Kill();
        return true;
    }

    public enum TunnelStatus
    {
        Failed,
        Succeed,
        AlreadyRunning
    }
}