# ChmlFrp.SDK.Extensions

一个为 ChmlFrp.SDK 提供相关扩展方法的扩展包.

### 安装

通过 NuGet 包安装 `ChmlFrp.SDK.Extensions`.

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.Extensions.svg)](https://www.nuget.org/packages/ChmlFrp.SDK.Extensions/)

```xml
<PackageReference Include="ChmlFrp.SDK.Extensions" Version="*"/>
```

### 使用

> 注意: 你需要先下载FRPC 客户端至当前目录，才能使用隧道相关功能.

隧道

```csharp
var forecast = await UserResult.AutoLoginAsync();

var tunnelResult = await forecast.GetTunnelResultAsync();
if (tunnelResult.State)
{
    var i = 1;
    foreach (var tunnel in tunnelResult.Data)
    {
        WriteLine($"{i}. {tunnel.Name}");
        i++;
    }

    forecast.StartTunnels(tunnelResult.Data,
        isStart => WriteLine(isStart.IsSuccess ? "启动FRPC成功" : "启动FRPC失败")
        new ()
        {
            LogFilePath = "frpc.log", // 自定义日志文件名
            FrpcFilePath = "frpc.exe", // 自定义FRPC文件名
            Arguments = "" // 自定义参数(启动命令后缀)
        });
}
else
{
    WriteLine(tunnelResult.Message);
}

ReadKey(true);

forecast.StopTunnels(tunnelResult.Data);
```