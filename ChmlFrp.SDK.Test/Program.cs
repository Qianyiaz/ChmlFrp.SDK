using ChmlFrp.SDK.Results;
using static System.Console;

var forecast = await UserResult.AutoLogin();
if (!forecast.State)
{
    while (true)
    {
        forecast = await UserResult.LoginAsync(ReadLine(), ReadLine());
        if (forecast == null)
            continue;
        WriteLine(forecast.Message);
        if (forecast.State) break;
    }
}

var user = forecast.Data;

/*
var nodes = await user.GetNodesAsync();
var i = 1;
foreach (var tunnel in nodes)
{
    WriteLine($"{i}. {tunnel.Name}");
    i++;
}
*/

var tunnels = await user.GetTunnelsAsync();
var i = 1;
foreach (var tunnel in tunnels)
{
    WriteLine($"{i}. {tunnel.Name}");
    // 你需要把FRPC文件放在当前执行目录才能启动
    // user.StartTunnel(tunnel, isStart => WriteLine(isStart == TunnelStatus.Succeed ? "启动FRPC成功" : "启动FRPC失败"));
    i++;
}

ReadKey();

// foreach (var tunnel in tunnels)
//    user.StopTunnel(tunnel);