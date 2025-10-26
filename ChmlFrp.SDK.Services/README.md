# ChmlFrp.SDK.Services

一个为 ChmlFrp.SDK 提供相关扩展方法的扩展包。

### 安装

通过 NuGet 包安装 `ChmlFrp.SDK.Services`。

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.Services.svg)](https://www.nuget.org/packages/ChmlFrp.SDK.Services/)

```
<PackageReference Include="ChmlFrp.SDK.Services" Version="*"/>
```

### 使用

隧道

```
var forecast = await UserResult.AutoLogin(); // 自动登录 前提已登录

var tunnelResult = await forecast.GetTunnelResultAsync();
if (tunnelResult.State)
{
    var i = 1;
    foreach (var tunnel in tunnelResult.Data)
    {
        WriteLine($"{i}. {tunnel.Name}");
        i++;
    }

    // 你需要把FRPC文件放在当前执行目录才能启动
    forecast.StartTunnels(tunnelResult.Data,
        isStart => WriteLine(isStart == TunnelServices.TunnelStatus.Succeed ? "启动FRPC成功" : "启动FRPC失败"));
}
else
{
    WriteLine(tunnelResult.Message);
}

ReadKey();

forecast.StopTunnels(tunnelResult.Data);
```

节点

```
var forecast = await UserResult.AutoLogin(); // 自动登录 前提已登录

var nodeResult = await forecast.GetNodeResultAsync();
if (nodeResult.State)
{
    var i = 1;
    foreach (var node in nodeResult.Data)
    {
        if (i == 1)
        {
            var nodeInfo = await forecast.GetNodeInfoResultAsync(node);
            WriteLine(nodeInfo.State ? nodeInfo.Data.Ip : nodeInfo.Message);
        }

        WriteLine($"{i}. {node.Name}");
        i++;
    }
}
else
{
    WriteLine(nodeResult.Message);
}
```

模板项目:https://github.com/Qianyiaz/ChmlFrp.SDK.AOT/blob/main/ChmlFrp.SDK.Test/Program.cs