# ChmlFrp.SDK.Extensions

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.Extensions.svg)](https://www.nuget.org/packages/ChmlFrp.SDK.Extensions/)

一个为 ChmlFrp.SDK 提供相关扩展方法的扩展包.

### 快速开始

> 注意: 你需要先下载FRPC 客户端至当前目录，才能使用隧道相关功能.

#### 启动隧道

```csharp
using ChmlFrp.SDK.Results;
using ChmlFrp.SDK.Extensions;

var tunnelResult = await userResult.GetTunnelResultAsync();
if (tunnelResult.State)
{
    void Handler(string line)
    {
        WriteLine(line);
    }
    
    var startResult = new TunnelServices.TunnelStartOptions
    {
        LogFilePath = "frpc.log", // 记录日志文件路径
        FrpcFilePath = "frpc.exe", // frpc文件路径 跨平台请使用frpc
        CommandSuffix = "", // 命令后缀
        Handler = Handler // 输出处理
    };

    // 启动单个隧道
    forecast.StartTunnel(tunnelResult.Data[0], startResult);

    // 启动多个隧道
    forecast.StartTunnels(tunnelResult.Data, startResult);
}
```

#### 停止隧道

```csharp
using ChmlFrp.SDK.Extensions;

// 停止单个隧道
userResult.StopTunnel(tunnelResult.Data[0]);

// 停止多个隧道
userResult.StopTunnels(tunnelResult.Data);
```

_更多用法与源码请查看仓库：[https://github.com/Qianyiaz/ChmlFrp.SDK/tree/main/src/ChmlFrp.SDK](https://github.com/Qianyiaz/ChmlFrp.SDK/tree/main/src/ChmlFrp.SDK)_