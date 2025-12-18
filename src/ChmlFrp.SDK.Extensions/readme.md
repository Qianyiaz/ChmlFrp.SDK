# ChmlFrp.SDK.Extensions

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.Extensions.svg)](https://www.nuget.org/packages/ChmlFrp.SDK.Extensions/)

为 ChmlFrp.SDK 提供隧道启动/停止等扩展方法的扩展包

> 注意: 你需要先准备好 frpc 可执行文件(frpc / frpc.exe),并确保 SDK 能够访问到该可执行文件,才能使用隧道相关功能

### 快速开始

先引用命名空间:

```csharp
using ChmlFrp.SDK.Services;
using ChmlFrp.SDK.Extensions;
using ChmlFrp.SDK.Models;
```

#### 启动隧道

- 默认 frpc 路径: AppDomain.CurrentDomain.BaseDirectory + "frpc"(可在 Windows 上放 frpc.exe或通过
  TunnelStartOptions.FrpcFilePath 设置完整路径)
- 在非 Windows 系统上 SDK 会尝试设置可执行权限(File.SetUnixFileMode)
- 日志默认写入临时文件(Path.GetTempFileName) 可通过 TunnelStartOptions.LogFilePath 指定

示例(启动单个隧道):

```csharp
// 先登录并获取用户信息(示例)
var userResult = await UserService.LoginAsync("username", "password");
if (userResult?.State != true) return;

// 获取隧道列表
var tunnelResult = await userResult.GetTunnelResponseAsync();
if (tunnelResult?.State != true) return;

// 输出处理回调
void Handler(string line) => Console.WriteLine(line);

// 启动选项
var startOptions = new TunnelServices.TunnelStartOptions
{
    LogFilePath = "frpc.log",     // 指定日志路径
    FrpcFilePath = "frpc.exe",    // Windows 上可用 frpc.exe；Linux/macOS 使用 "frpc"或 (完整路径)
    CommandSuffix = null,         // 命令后缀(默认为空,建议别更改)
    Handler = Handler             // 输出行处理
};

// 启动第一个隧道
userResult.StartTunnel(tunnelResult.Data![0], startOptions);
```

示例(启动多个隧道):

```csharp
// 将多个隧道的 id 传递给同一个 frpc 进程
userResult.StartTunnel(tunnelResult.Data!, startOptions);
```

内部调用构建的 frpc 启动参数类似:
Arguments = options?.CommandSuffix ?? $"-u {user.Data!.UserToken} -p {id}";

并且会把 frpc 标准输出追加到指定日志文件 同时把每一行传给 Handler 回调(如果提供的话)

#### 停止隧道

示例(停止单个隧道):

```csharp
userResult.StopTunnel(tunnelResult.Data![0]);
```

示例(停止多个隧道):

```csharp
userResult.StopTunnel(tunnelResult.Data!);
```

StopTunnel 会尝试获取并 Kill 对应的 frpc 进程(若该隧道当前有运行的进程)

#### 隧道进程管理(API 说明)

- TunnelData 的扩展(通过 ConditionalWeakTable 关联 Process):
    - tunnel.SetFrpProcess(Process) — 记录并关联隧道对应的 Process
    - tunnel.GetFrpProcess() — 获取关联的 Process(如果存在)
    - tunnel.IsRunning() — 判断关联的 Process 是否存在且未退出

#### 注意事项

- 请确保提供的 FrpcFilePath 指向有效的可执行文件；在非 Windows 平台建议指定名称为 "frpc" 或完整路径,并确保可执行权限,SDK
  会在非 Windows 平台调用 File.SetUnixFileMode 以尝试设置执行权限,但这需要运行进程有相应权限
- LogFilePath 如果不指定将使用系统临时文件(可能每次运行不同),建议指定稳定路径以便查看历史日志

更多用法与源码请查看仓库:
https://github.com/Qianyiaz/ChmlFrp.SDK/tree/main/src/ChmlFrp.SDK.Extensions