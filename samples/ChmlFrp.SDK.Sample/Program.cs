using ChmlFrp.SDK.Extensions;
using ChmlFrp.SDK.Services;
using static System.Console;

var forecast = await UserService.AutoLoginAsync(); // 尝试自动登录
if (!forecast!.State) // 自动登录失败 进行手动登录
    while (true)
    {
        Clear();
        Write("用户名: ");
        var userName = ReadLine();

        Write("密码: ");
        var password = ReadLine();
        forecast = await UserService.LoginAsync(userName, password); // 登录

        WriteLine(forecast?.Message); // 显示登录结果消息
        if (forecast!.State)
            break; // 登录成功 跳出循环
    }

// 显示用户信息
var nodeResult = await forecast.GetNodeResponseAsync(); // 获取节点列表
if (nodeResult!.State)
{
    var i = 1;
    foreach (var node in nodeResult.Data!)
    {
        if (i == 1)
        {
            var nodeInfo = await forecast.GetNodeInfoResponseAsync(node);
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

var tunnelResult = await forecast.GetTunnelResponseAsync(); // 获取隧道列表

if (tunnelResult!.State)
{
    var i = 1;
    foreach (var tunnel in tunnelResult.Data!)
    {
        WriteLine($"{i}. {tunnel.Name}");
        i++;
    }

    WriteLine("点击启动隧道");
    ReadKey(true);
    Clear();

    forecast.StartTunnel(tunnelResult.Data, new()
    {
        Handler = WriteLine
    }); // 启动所有隧道 并显示启动结果
    // 注意 启动FRPC需要本地已配置好FRPC环境
}
else
{
    WriteLine(tunnelResult.Message);
    return;
}

ReadKey(true);

if (tunnelResult.Data != null)
    forecast.StopTunnel(tunnelResult.Data); // 停止所有隧道