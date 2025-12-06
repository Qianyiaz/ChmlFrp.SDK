using System.Diagnostics;
using System.Runtime.CompilerServices;
using ChmlFrp.SDK.Results;

namespace ChmlFrp.SDK.Extensions;

/// <summary>
/// 隧道进程相关的操作
/// </summary>
public static class TunnelProcess
{
    private static readonly ConditionalWeakTable<TunnelData, Process> ProcessInfos = new();

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
            ProcessInfos.AddOrUpdate(tunnel, process);
        }

        /// <summary>
        /// 获取进程
        /// </summary>
        /// <returns>设置的进程</returns>
        public Process? GetFrpProcess()
        {
            return ProcessInfos.TryGetValue(tunnel, out var process) ? process : null;
        }

        /// <summary>
        /// 获得隧道的进程是否在运行
        /// </summary>
        /// <returns>隧道的进程是否在运行</returns>
        public bool IsRunning()
        {
            return tunnel.GetFrpProcess() is { HasExited: false };
        }
    }
}