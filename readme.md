# ChmlFrp.SDK

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.svg)](https://www.nuget.org/packages/ChmlFrp.SDK/)

为 .NET 开发者提供的第三方 ChmlFrp 客户端开发工具包,包含与服务端交互的请求与响应类型封装

> 注意: 示例中的异步方法都返回对应的 Response 类型(例如 `UserResponse`,`TunnelResponse` 等),并且大多数响应有一个布尔属性
`State` 用于判断请求是否成功 Data 字段包含具体的数据模型

## 快速开始

先引用命名空间:

```csharp
using ChmlFrp.SDK.Services;
using ChmlFrp.SDK.Responses;
using ChmlFrp.SDK.Models;
```

### 用户登录

使用用户名和密码登录(登录成功时会默认保存令牌到应用数据目录除非传入 saveToken: false):

```csharp
var userResult = await UserService.LoginAsync("username", "password");
if (userResult?.State == true)
{
    Console.WriteLine($"登录成功,欢迎 {userResult.Data?.Username}");
}
else
{
    Console.WriteLine($"登录失败: {userResult?.Message}");
}
```

使用已保存的令牌登录(或直接传入 token):

```csharp
// 自动从 AppData 下的 ChmlFrp/user.json 读取保存的 token 并登录
var autoLoginResult = await UserService.AutoLoginAsync();
if (autoLoginResult?.State == true)
{
    Console.WriteLine("自动登录成功");
}

// 直接使用令牌登录
var tokenLogin = await UserService.LoginByTokenAsync("your-token-here");
```

登出(删除本地保存的令牌):

```csharp
// 假设 userResult 是已登录的 UserResponse
await userResult.LoginoutAsync();
```

### 获取隧道信息

获取当前用户的隧道列表:

```csharp
var tunnelResult = await userResult.GetTunnelResponseAsync();
if (tunnelResult?.State == true)
{
    foreach (var tunnel in tunnelResult.Data!)
    {
        Console.WriteLine($"隧道: {tunnel.Name}");
        Console.WriteLine($"类型: {tunnel.Type}");
        Console.WriteLine($"是否在线: {tunnel.State}");
        Console.WriteLine($"远程地址: {tunnel.FullRemoteAddress}");
    }
}
else
{
    Console.WriteLine($"获取隧道失败: {tunnelResult?.Message}");
}
```

### 创建隧道

获取当前用户的隧道列表:

```csharp
var createRequest = new CreateTunnelRequest
{
    TunnelName = "my-tcp-tunnel",
    Node = "node-name",
    PortType = "tcp", // 例如:tcp/udp/http/https (必须小写)
    LocalIp = "127.0.0.1",
    LocalPort = 3389,
    RemotePort = 10000,
    BandDomain = "your-domain-name",
    Encryption = false
    Compression = false,
    ExtraParams = ""
};

var createResult = await TunnelService.CreateTunnelAsync(createRequest);
```

### 获取节点信息

获取节点列表:

```csharp
var nodeResult = await userResult.GetNodeResponseAsync();
if (nodeResult?.State == true)
{
    foreach (var node in nodeResult.Data!)
    {
        Console.WriteLine($"节点: {node.Name}");
        Console.WriteLine($"地区: {node.Area}");
        Console.WriteLine($"是否在中国: {node.IsInChina}");
    }
}
```

获取单个节点详情(传入 NodeData):

```csharp
// 假设有一个 node 对象来自 nodeResult.Data
var nodeInfoResult = await userResult.GetNodeInfoResponseAsync(node);
if (nodeInfoResult?.State == true)
{
    var nodeInfo = nodeInfoResult.Data!;
    Console.WriteLine($"CPU 核心数: {nodeInfo.NumCores}");
    Console.WriteLine($"内存总量: {nodeInfo.MemoryTotalGB:F2} GB");
    Console.WriteLine($"今日下载: {nodeInfo.TotalTrafficInGB:F2} GB");
}
```

### 其他用户相关操作

重置用户 token:

```csharp
var resetResult = await userResult.ResetTokenAsync();
Console.WriteLine(resetResult?.State == true ? "重置成功" : $"重置失败: {resetResult?.Message}");
```

更新用户 QQ:

```csharp
var updateQqResult = await userResult.UpdateQqAsync("123456789");
```

更新用户名:

```csharp
var updateNameResult = await userResult.UpdateNameAsync("newname");
```

## 保存的令牌位置

SDK 会把令牌保存为 JSON 文件路径为:

- Windows: %APPDATA%/ChmlFrp/user.json
- MacOS: $HOME/Library/Application Support/ChmlFrp/user.json
- Linux: 通常是 $HOME/.config/ChmlFrp/user.json

## 响应与模型简介

常见响应类型:

- BaseResponse: 基础响应,包含 Message 与 State(字符串->布尔的封装)
- UserResponse: 包含 UserData(用户详细信息)
- TunnelResponse: 包含隧道数据列表(TunnelData)
- NodeResponse: 包含节点列表(NodeData)
- NodeInfoResponse: 包含单个节点详情(NodeInfo)
- CreateTunnelResponse: 包含单个隧道数据(TunnelData)

常用模型示例字段:

- TunnelData.FullRemoteAddress: 根据隧道类型生成的完整远程地址(HTTP/HTTPS/TCP/UDP)
- NodeInfo.MemoryTotalGB / StorageAvailableGB / UptimeHours 等便捷只读属性,已做单位换算

## 进一步阅读

源码与更多示例请查看仓库:
https://github.com/Qianyiaz/ChmlFrp.SDK/tree/main/src/ChmlFrp.SDK

如果你打算使用隧道的启动/停止等功能,请安装并查看 [ChmlFrp.SDK.Extensions](https://www.nuget.org/packages/ChmlFrp.SDK.Extensions)
包(独立扩展包)