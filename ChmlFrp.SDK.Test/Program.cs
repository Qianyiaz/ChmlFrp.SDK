using ChmlFrp.SDK.Results;
using static System.Console;

var forecast = await UserResult.AutoLogin();
if (!forecast.State)
{
    while (true)
    {
        forecast = await UserResult.LoginAsync(ReadLine(), ReadLine());
        WriteLine(forecast.Message);
        if (forecast.State)
            break;
    }
}

var user = forecast.Data;

/*
var nodeResult = await user.GetNodeResultAsync();
if (nodeResult.State)
{
    var i = 1;
    foreach (var node in nodeResult.Data)
    {
        WriteLine($"{i}. {node.Name}");
        i++;
    }
}
else
{
    WriteLine(nodeResult.Message);
}
*/

var tunnelResult = await user.GetTunnelResultAsync();
if (tunnelResult.State)
{
    var i = 1;
    foreach (var tunnel in tunnelResult.Data)
    {
        WriteLine($"{i}. {tunnel.Name}");
        // 你需要把FRPC文件放在当前执行目录才能启动
        // user.StartTunnel(tunnel, isStart => WriteLine(isStart == TunnelStatus.Succeed ? "启动FRPC成功" : "启动FRPC失败"));
        i++;
    }
}
else
{
    WriteLine(tunnelResult.Message);
}

ReadKey();

// foreach (var tunnel in tunnels)
//    user.StopTunnel(tunnel);