using ChmlFrp.SDK.Extensions;
using ChmlFrp.SDK.Results;
using static System.Console;

var forecast = await UserResult.AutoLoginAsync(); // 尝试自动登录
if (!forecast!.State) // 自动登录失败，进行手动登录
    while (true)
    {
        Write("用户名: ");
        var userName = ReadLine();
        
        Write("密码: ");
        var password = ReadLine();
        forecast = await UserResult.LoginAsync(userName, password); // 登录

        WriteLine(forecast?.Message); // 显示登录结果消息
        if (forecast!.State)
            break; // 登录成功，跳出循环
    }

// 显示用户信息
var nodeResult = await forecast.GetNodeResultAsync(); // 获取节点列表
if (nodeResult!.State)
{
    var i = 1;
    foreach (var node in nodeResult.Data!)
    {
        if (i == 1)
        {
            var nodeInfo = await forecast.GetNodeInfoResultAsync(node);
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

var tunnelResult = await forecast.GetTunnelResultAsync(); // 获取隧道列表
if (tunnelResult!.State)
{
    var i = 1;
    foreach (var tunnel in tunnelResult.Data!)
    {
        WriteLine($"{i}. {tunnel.Name}");
        i++;
    }

    forecast.StartTunnels(
        tunnelResult.Data,
        isStart =>
            WriteLine(isStart.IsSuccess ? "启动FRPC成功" : "启动FRPC失败")); // 启动所有隧道，并显示启动结果
    // 注意：启动FRPC需要本地已配置好FRPC环境
}
else
{
    WriteLine(tunnelResult.Message);
}

ReadKey(true);

if (tunnelResult.Data != null) 
    forecast.StopTunnels(tunnelResult.Data); // 停止所有隧道