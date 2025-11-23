# ChmlFrp.SDK

一个为 .NET 开发者提供的第三方 ChmlFrp 客户端开发工具包。

### 安装

通过 NuGet 包安装 `ChmlFrp.SDK`。

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.svg)](https://www.nuget.org/packages/ChmlFrp.SDK/)

```xml
<PackageReference Include="ChmlFrp.SDK" Version="*"/>
```

### 使用

登录

```csharp
var forecast = await UserResult.LoginAsync(username, password);
WriteLine(forecast.Message);
```

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
}
else
{
    WriteLine(tunnelResult.Message);
}
```

节点

```csharp
var forecast = await UserResult.AutoLoginAsync();
var nodeResult = await forecast!.GetNodeResultAsync();
if (nodeResult?.State == true)
{
    var i = 1;
    foreach (var node in nodeResult.Data!)
    {
        if (i == 1)
        {
            var nodeInfo = await forecast.GetNodeInfoResultAsync(node);
            WriteLine(nodeInfo!.State ? nodeInfo.Data!.Ip : nodeInfo.Message);
        }

        WriteLine($"{i}. {node.Name}");
        i++;
    }
}
else
{
    WriteLine(nodeResult!.Message);
}
```