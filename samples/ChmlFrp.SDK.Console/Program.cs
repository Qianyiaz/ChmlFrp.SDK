using ChmlFrp.SDK.Extensions;
using ChmlFrp.SDK.Models;
using ChmlFrp.SDK.Service;
using static System.Console;

var client = new ChmlFrpClient();

var forecast = await client.AutoLoginAsync(); // 尝试自动登录
if (!forecast!.State) // 自动登录失败 进行手动登录
    while (true)
    {
        Clear();
        Write("用户名: ");
        var userName = ReadLine();

        Write("密码: ");
        var password = ReadLine();
        forecast = await client.LoginAsync(userName, password); // 登录

        WriteLine(forecast?.Message); // 显示登录结果消息
        ReadKey(true);
        
        if (forecast!.State)
            break; // 登录成功 跳出循环 
    }

var createReq = new UpdateTunnelRequest
{
    TunnelName = "mytesttunnel",
    Node = "成都电信",
    PortType = "tcp",
    LocalIp = "127.0.0.1",
    LocalPort = 8080,
    RemotePort = 67422
    // RemotePort, BandDomain, Encryption, Compression, ExtraParams 可按需设置
};

var createResult = await client.UpdateTunnelAsync(createReq);
if (createResult!.State)
{
    WriteLine("创建隧道成功: " + createResult.Data?.Name);
}
else
{
    WriteLine("创建隧道失败: " + createResult.Message);
}

// 显示用户信息
var nodeResult = await client.GetNodeResponseAsync(); // 获取节点列表
if (nodeResult!.State)
{
    var i = 1;
    foreach (var node in nodeResult.Data!)
    {
        if (i == 1)
        {
            var nodeInfo = await client.GetNodeInfoResponseAsync(node);
            WriteLine(nodeInfo!.State ? nodeInfo.Data!.Ip : nodeInfo.Message);
        } // 显示第一个节点的IP地址

        WriteLine($"{i}. {node.Name}");
        i++;
    }
}
else
{
    WriteLine(nodeResult.Message);
}

var tunnelResult = await client.GetTunnelResponseAsync(); // 获取隧道列表

if (tunnelResult!.State)
{
    if (tunnelResult.Data!.Count == 0)
    {
        WriteLine("暂无隧道");
        ReadKey(true); 
        return;
    }
    
    var i = 1;
    foreach (var tunnel in tunnelResult.Data!)
    {
        WriteLine($"{i}. {tunnel.Name}");
        i++;
    }

    WriteLine("点击启动隧道");
    ReadKey(true);
    Clear();

    try
    {
        client.StartTunnel(tunnelResult.Data, new()
        {
            Handler = WriteLine
        }); // 启动所有隧道 并显示启动结果
        // 注意 启动FRPC需要本地已配置好FRPC环境
    }
    catch (Exception e)
    {
        WriteLine(e.Message);
    }
}
else 
{
    WriteLine(tunnelResult.Message);
    ReadKey(true); 
    return;
}

ReadKey(true);

client.StopTunnel(tunnelResult.Data); // 停止所有隧道