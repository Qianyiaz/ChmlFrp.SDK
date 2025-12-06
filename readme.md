# ChmlFrp.SDK

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.svg)](https://www.nuget.org/packages/ChmlFrp.SDK/)

一个为 .NET 开发者提供的第三方 ChmlFrp 客户端开发工具包。

### 快速开始

#### 用户登录

```csharp
using ChmlFrp.SDK.Results;

// 使用用户名密码登录
var userResult = await UserResult.LoginAsync("username", "password");
if (userResult.State)
{
    Console.WriteLine($"登录成功，欢迎 {userResult.Data.Username}");
}

// 自动登录(使用保存的令牌)
var autoLoginResult = await UserResult.AutoLoginAsync();
if (autoLoginResult.State)
{
    Console.WriteLine("自动登录成功");
}
```

#### 获取隧道信息

> 不包含启动和停止隧道功能，请安装 [ChmlFrp.SDK.Extensions](https://www.nuget.org/packages/ChmlFrp.SDK.Extensions) 包.

```csharp
using ChmlFrp.SDK.Results;

var tunnelResult = await userResult.GetTunnelResultAsync();
if (tunnelResult.State)
{
    foreach (var tunnel in tunnelResult.Data)
    {
        Console.WriteLine($"隧道: {tunnel.Name}");
        Console.WriteLine($"类型: {tunnel.Type}");
        Console.WriteLine($"状态: {tunnel.State}");
        Console.WriteLine($"远程地址: {tunnel.FullRemoteAddress}");
    }
}
```

#### 获取节点信息

```csharp
using ChmlFrp.SDK.Results;

var nodeResult = await userResult.GetNodeResultAsync();
if (nodeResult.State)
{
    foreach (var node in nodeResult.Data)
    {
        Console.WriteLine($"节点: {node.Name}");
        Console.WriteLine($"地区: {node.Area}");
        Console.WriteLine($"在线状态: {node.State}");
    }
}

// 获取节点详情
var nodeInfoResult = await userResult.GetNodeInfoResultAsync(node);
if (nodeInfoResult.State)
{
    var nodeInfo = nodeInfoResult.Data;
    Console.WriteLine($"CPU核心: {nodeInfo.NumCores}");
    Console.WriteLine($"内存总量: {nodeInfo.MemoryTotalGB} GB");
    Console.WriteLine($"今日流量: {nodeInfo.TotalTrafficInGB} GB");
}
```

_更多用法与源码请查看仓库：[https://github.com/Qianyiaz/ChmlFrp.SDK/tree/main/src/ChmlFrp.SDK.Extensions](https://github.com/Qianyiaz/ChmlFrp.SDK/tree/main/src/ChmlFrp.SDK.Extensions)_