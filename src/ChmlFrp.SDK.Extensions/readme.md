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
    void Handler(string line)
    {
        WriteLine(line);
    }

    // 启动单个隧道
    forecast.StartTunnels(tunnelResult.Data,new ()
    {
        LogFilePath = "frpc.log", // 记录日志文件路径
        FrpcFilePath = "frpc.exe", // frpc文件路径
        CommandSuffix = "", // 命令后缀
        Handler = Handler // 输出处理
    });

    // 启动多个隧道
    forecast.StartTunnel(tunnelResult.Data,new ()
    {
        LogFilePath = "frpc.log", // 记录日志文件路径
        FrpcFilePath = "frpc.exe", // frpc文件路径
        CommandSuffix = "", // 命令后缀
        Handler = Handler // 输出处理
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