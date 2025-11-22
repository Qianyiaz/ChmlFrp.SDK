using ChmlFrp.SDK.Results;
using ChmlFrp.SDK.Extensions;
using static System.Console;

var forecast = await UserResult.AutoLogin();
if (!forecast?.State == true)
    while (true)
    {
        forecast = await UserResult.LoginAsync(ReadLine(), ReadLine());
        WriteLine(forecast?.Message);
        if (forecast?.State == true)
            break;
    }


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

var tunnelResult = await forecast.GetTunnelResultAsync();
if (tunnelResult?.State == true)
{
    var i = 1;
    foreach (var tunnel in tunnelResult.Data!)
    {
        WriteLine($"{i}. {tunnel.Name}");
        i++;
    }

    // 你需要把FRPC文件放在当前执行目录才能启动
    forecast.StartTunnels(tunnelResult.Data,
        isStart => WriteLine(isStart.IsSuccess ? "启动FRPC成功" : "启动FRPC失败"));
}
else
{
    WriteLine(tunnelResult!.Message);
}

ReadKey();

forecast.StopTunnels(tunnelResult.Data!);
tunnelResult.Data!.ForEach(tunnel => WriteLine(tunnel.IsRunning()));