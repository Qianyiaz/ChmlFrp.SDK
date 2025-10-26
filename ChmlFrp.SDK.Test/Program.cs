using ChmlFrp.SDK.Results;
using ChmlFrp.SDK.Services;
using static System.Console;

_ = Task.Run(BaseResult.WarmUpConnectionAsync);

var forecast = await UserResult.AutoLogin();
if (!forecast.State)
    while (true)
    {
        forecast = await UserResult.LoginAsync(ReadLine(), ReadLine());
        WriteLine(forecast.Message);
        if (forecast.State)
            break;
    }


var nodeResult = await forecast.GetNodeResultAsync();
if (nodeResult.State)
{
    var i = 1;
    foreach (var node in nodeResult.Data)
    {
        if (i == 1)
        {
            var nodeInfo = await forecast.GetNodeInfoResultAsync(node);
            WriteLine(nodeInfo.State ? nodeInfo.Data.Ip : nodeInfo.Message);
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
if (tunnelResult.State)
{
    var i = 1;
    foreach (var tunnel in tunnelResult.Data)
    {
        WriteLine($"{i}. {tunnel.Name}");
        i++;
    }

    // 你需要把FRPC文件放在当前执行目录才能启动
    forecast.StartTunnels(tunnelResult.Data,
        isStart => WriteLine(isStart == TunnelServices.TunnelStatus.Succeed ? "启动FRPC成功" : "启动FRPC失败"));
}
else
{
    WriteLine(tunnelResult.Message);
}

ReadKey();

forecast.StopTunnels(tunnelResult.Data);