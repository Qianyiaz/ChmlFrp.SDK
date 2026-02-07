# ChmlFrp.SDK.Extensions

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.Extensions.svg)](https://www.nuget.org/packages/ChmlFrp.SDK.Extensions/)

为 ChmlFrp.SDK 提供隧道启动/停止等扩展方法的扩展包。

> 注意：使用隧道相关功能前，请先准备好 frpc 可执行文件（frpc / frpc.exe），并确保 SDK 能够访问到该可执行文件。

## 快速开始

先引用命名空间：

```csharp
using ChmlFrp.SDK.Extensions;
using ChmlFrp.SDK.Service; // 若需直接使用 ChmlFrpClient
```

### 启动隧道（单个或多个）

- 默认 frpc 路径：`AppDomain.CurrentDomain.BaseDirectory + "frpc"`（Windows 上可放 `frpc.exe`，也可通过
  TunnelStartOptions.FrpcFilePath 指定完整路径）
- 在非 Windows 系统上 SDK 会尝试设置可执行权限（使用 `File.SetUnixFileMode`）
- 日志默认写入临时文件（`Path.GetTempFileName()`），可通过 `TunnelStartOptions.LogFilePath` 指定

示例（启动单个隧道）：

```csharp
var client = new ChmlFrpClient();

// 登录（示例）
var loginResult = await client.LoginAsync("username", "password");
if (loginResult?.State != true) return;

// 获取隧道列表
var tunnelResult = await client.GetTunnelResponseAsync();
if (tunnelResult?.State != true) return;

// 输出处理回调
void Handler(string line) => Console.WriteLine(line);

// 启动选项
var startOptions = new TunnelServiceExtensions.TunnelStartOptions
{
    IsUseLogFile = true,           // 启用日志文件 （默认 true）
    LogFilePath = "frpc.log",      // 指定日志路径
    FrpcFilePath = "frpc.exe",     // Windows 上常用, Linux/macOS 使用 "frpc" 或完整路径
    CommandSuffix = null,          // 若为 null 则使用默认 "-u %token% -p %id%"
    Handler = Handler              // 输出行处理
};

// 启动第一个隧道（StartTunnel 为 ChmlFrpClient 的扩展方法）
client.StartTunnel(tunnelResult.Data![0], startOptions);
```

示例（启动多个隧道——将多个隧道的 id 传给同一个 frpc 进程）：

```csharp
client.StartTunnel(tunnelResult.Data!, startOptions);
```

内部构建的 frpc 启动参数默认形式为：

```
-u %token% -p %id%
```

SDK 会把 `%token%` 替换为当前用户 token，把 `%id%` 替换为隧道 id（若传入多个隧道则为逗号分隔的 id 列表）。

启动后，frpc 的标准输出会追加到指定日志文件，同时每一行会回调给 `Handler`（若提供）。

### 停止隧道

示例（停止单个隧道）：

```csharp
client.StopTunnel(tunnelResult.Data![0]);
```

示例（停止多个隧道）：

```csharp
client.StopTunnel(tunnelResult.Data!);
```

StopTunnel 会尝试获取并 Kill 对应的 frpc 进程（若该隧道当前有运行的进程）。在支持的平台上会尝试以递归方式结束子进程（视 .NET
版本决定行为）。

### 隧道进程管理（API 说明）

通过 ConditionalWeakTable 将 Process 与 TunnelData 关联，提供以下扩展方法：

- tunnel.SetFrpProcess(Process) — 记录并关联隧道对应的 Process（内部 API）
- tunnel.GetFrpProcess() — 获取关联的 Process（如果存在）
- tunnel.IsRunning() — 判断关联的 Process 是否存在且未退出

这些方法用于管理同一个 frpc 进程或为每个隧道记录其运行状态。

### 注意事项与建议

- 请确保 `FrpcFilePath` 指向有效的可执行文件；在非 Windows 平台建议使用名称为 `frpc` 或完整路径，并确保可执行权限。
- `File.SetUnixFileMode` 需要运行进程具有相应权限；如果权限不足，建议手动为 frpc 添加执行权限（`chmod +x frpc`）。
- `LogFilePath` 如果不指定将使用系统临时文件（不同运行可能不同）。建议指定稳定路径以便查看历史日志。
- 当一次启动多个隧道时，SDK 会把多个 id 以逗号连接传递给同一个 frpc 进程，日志与回调也会集中到同一进程上。
- 其他系统暂时没有测试（除Windows外）可能会影响行为，建议在使用前测试相关功能。

更多用法与源码请查看仓库：
https://github.com/Qianyiaz/ChmlFrp.SDK/tree/main/src/ChmlFrp.SDK.Extensions