# ChmlFrp.SDK.Extensions

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.Extensions.svg)](https://www.nuget.org/packages/ChmlFrp.SDK.Extensions/)

一个为 ChmlFrp.SDK 提供相关扩展方法的扩展包.

### 用法

> 注意: 你需要先下载FRPC 客户端至当前目录，才能使用隧道相关功能.

#### 启动隧道

```csharp
var tunnelResult = await userResult.GetTunnelResultAsync();
if (tunnelResult.State)
{
    // 启动单个隧道
    userResult.StartTunnel(tunnelResult.Data[0], 
        status => Console.WriteLine(status.IsSuccess ? "启动成功" : "启动失败"),
        new TunnelServices.TunnelStartOptions
        {
            LogFilePath = "frpc.log", // 自定义日志文件路径
            FrpcFilePath = "frpc.exe", // 自定义FRPC文件路径
            Arguments = "" // 自定义参数(启动命令后缀)
        });

    // 启动多个隧道
    userResult.StartTunnels(tunnelResult.Data,
        status => Console.WriteLine(status.Message),
        new TunnelServices.TunnelStartOptions
        {
            LogFilePath = "frpc.log", // 自定义日志文件路径
            FrpcFilePath = "frpc.exe", // 自定义FRPC文件路径
            Arguments = "" // 自定义参数(启动命令后缀)
        });
}
```

#### 停止隧道

```csharp
// 停止单个隧道
userResult.StopTunnel(tunnelResult.Data[0]);

// 停止多个隧道
userResult.StopTunnels(tunnelResult.Data);
```

_更多功能请自行查看 [源代码仓库](https://github.com/Qianyiaz/ChmlFrp.SDK)._