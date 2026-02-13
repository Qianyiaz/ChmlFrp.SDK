# ChmlFrp.SDK

[![NuGet](https://img.shields.io/nuget/v/ChmlFrp.SDK.svg)](https://www.nuget.org/packages/ChmlFrp.SDK/)

为 .NET 开发者提供的第三方 ChmlFrp 客户端开发工具包，包含与服务端交互的请求与响应类型封装以及常用的用户/隧道/节点相关
API。

> 注意：示例中的异步方法都返回对应的 Response 类型（例如 `DataResponse<T>` 等），大多数响应包含一个布尔属性 `State`
> 用于判断请求是否成功，`Data` 字段包含具体的数据模型.
> 而且大多数异步方法可能会throw,建议加上try,catch调用.

## 快速开始

先引用命名空间：

```csharp
using ChmlFrp.SDK.Service;
using ChmlFrp.SDK.Models;
```

示例通常以 `ChmlFrpClient` 为入口：

```csharp
var client = new ChmlFrpClient();
```

### 登录

使用用户名和密码登录（登录成功时会默认保存令牌到应用数据目录，除非传入 `saveToken: false`）：

```csharp
var loginResult = await client.LoginAsync("username", "password");
if (loginResult?.State == true)
{
    Console.WriteLine($"登录成功，欢迎 {loginResult.Data?.Username}");
}
else
{
    Console.WriteLine($"登录失败: {loginResult?.Message}");
}
```

使用已保存的令牌自动登录：

```csharp
// 从应用数据目录读取保存的 token 并尝试登录
var autoLoginResult = await client.AutoLoginAsync();
if (autoLoginResult?.State == true)
{
    Console.WriteLine("自动登录成功");
}
else
{
    Console.WriteLine($"自动登录失败: {autoLoginResult?.Message}");
}
```

使用显式 token 登录：

```csharp
var tokenLoginResult = await client.LoginByTokenAsync("your-token-here");
```

本地“登出”（删除本地保存的 token）可以通过删除保存的文件来实现：

```csharp
// 删除本地保存的 token 文件（路径参见下方“保存的令牌位置”）
File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChmlFrp", "user.json"));
```

### 获取隧道信息

获取当前用户的隧道列表：

```csharp
var tunnelResult = await client.GetTunnelResponseAsync();
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

> 提示：如需在本机启动/停止隧道，请安装并使用独立扩展包 [ChmlFrp.SDK.Extensions](https://www.nuget.org/packages/ChmlFrp.SDK.Extensions)。

### 创建隧道：

```csharp
var createReq = new CreateTunnelRequest()
{
    TunnelName = "test-tunnel",
    Node = "node1", // 需要替换为实际的节点名称
    PortType = "tcp", // tcp/udp/http/https
    LocalIp = "127.0.0.1",
    LocalPort = 8080,
    RemotePort = 12345
    // RemotePort, BandDomain, Encryption, Compression, ExtraParams 可按需设置
};

var createResult = await client.CreateTunnelAsync(createReq);
if (createResult?.State == true)
{
    WriteLine("创建隧道成功: " + createResult.Data?.Name);
}
else
{
    WriteLine("创建隧道失败: " + createResult.Message);
}
```

### 更新隧道：

```csharp
var updateReq = new UpdateTunnelRequest()
{
    TunnelId = 12345, // 隧道ID
    TunnelName = "test-tunnel",
    Node = "node1", // 需要替换为实际的节点名称
    PortType = "tcp", // tcp/udp/http/https
    LocalIp = "127.0.0.1",
    LocalPort = 8080,
    RemotePort = 12345
    // RemotePort, BandDomain, Encryption, Compression, ExtraParams 可按需设置
};

var updateResult = await client.UpdateTunnelAsync(updateReq);
if (updateResult?.State == true)
{
    WriteLine("更新隧道成功: " + updateResult.Data?.Name);
}
else
{
    WriteLine("更新隧道失败: " + updateResult.Message);
}
```

### 获取节点信息

获取节点列表：

```csharp
var nodeResult = await client.GetNodeResponseAsync();
if (nodeResult?.State == true)
{
    foreach (var node in nodeResult.Data!)
    {
        Console.WriteLine($"节点: {node.Name}");
        Console.WriteLine($"地区: {node.Area}");
        Console.WriteLine($"是否在中国: {node.IsInChina}");
    }
}
else
{
    Console.WriteLine($"获取节点失败: {nodeResult?.Message}");
}
```

获取单个节点详情（传入 `NodeData`）：

```csharp
// 假设有一个 node 对象来自 nodeResult.Data
var nodeInfoResult = await client.GetNodeInfoResponseAsync(node);
if (nodeInfoResult?.State == true)
{
    var nodeInfo = nodeInfoResult.Data!;
    Console.WriteLine($"CPU 核心数: {nodeInfo.NumCores}");
}
```

### 其他用户相关操作

重置用户服务器端 token（会在服务端生成新 token）：

```csharp
var resetResult = await client.ResetTokenAsync();
Console.WriteLine(resetResult?.State == true ? "重置成功" : $"重置失败: {resetResult?.Message}");
```

更新用户 QQ：

```csharp
var updateQqResult = await client.UpdateQQAsync("123456789");
```

更新用户名：

```csharp
var updateNameResult = await client.UpdateNameAsync("newname");
```

## 保存的令牌位置

SDK 默认会把令牌保存为 JSON 文件，路径为：

- Windows: %APPDATA%/ChmlFrp/user.json
- MacOS: $HOME/Library/Application Support/ChmlFrp/user.json
- Linux: 通常是 $HOME/.config/ChmlFrp/user.json

文件示例内容大致如下：

```json
{ "usertoken": "xxxxx" }
```

AutoLoginAsync 会尝试读取该文件并用 token 登录；如果文件不存在或 token 无效，会返回失败信息。

## 关于隧道启动/停止（可选扩展）

如果你需要在本地启动/停止 frpc 以运行隧道，请安装并使用独立扩展包：

- NuGet: https://www.nuget.org/packages/ChmlFrp.SDK.Extensions
- 扩展提供：启动/停止隧道、将 frpc 进程与 TunnelData 关联、日志回调等功能。

扩展的一些要点：

- 需要本地提供 frpc 可执行文件（Windows: frpc.exe；Unix: frpc 且有可执行权限）。
- 默认命令后缀为 `-u %token% -p %id%`，扩展会将 `%token%`、`%id%` 替换为真实值。
- 日志会写入临时文件，且每行输出会回调到用户提供的处理程序。

## 进一步阅读

源码与更多示例请查看仓库：
https://github.com/Qianyiaz/ChmlFrp.SDK/tree/main/src/ChmlFrp.SDK
