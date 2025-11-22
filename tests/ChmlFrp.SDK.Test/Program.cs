using ChmlFrp.SDK.Extensions;
using ChmlFrp.SDK.Results;
using static System.Console;

var forecast = await UserResult.AutoLoginAsync();
if (!forecast!.State)
    while (true)
    {
        Write("用户名: ");
        var userName = ReadLine();
        Write("密码: ");
        var password = ReadLine();
        forecast = await UserResult.LoginAsync(userName, password);

        WriteLine(forecast?.Message);
        if (forecast!.State)
            break;
    }


var nodeResult = await forecast.GetNodeResultAsync();
if (nodeResult!.State)
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
    WriteLine(nodeResult.Message);
}

var tunnelResult = await forecast.GetTunnelResultAsync();
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
            WriteLine(isStart.IsSuccess ? "启动FRPC成功" : "启动FRPC失败"));
}
else
{
    WriteLine(tunnelResult.Message);
}

ReadKey(true);

forecast.StopTunnels(tunnelResult.Data!);
tunnelResult.Data!.ForEach(tunnel => WriteLine(tunnel.IsRunning()));