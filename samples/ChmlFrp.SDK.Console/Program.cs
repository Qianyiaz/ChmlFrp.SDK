using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;
using ChmlFrp.SDK.Service;
using static System.Console;

using var client = new ChmlFrpClient();

DataResponse<UserData>? autoLoginAsync = null;
try
{
    autoLoginAsync = await client.AutoLoginAsync();
}
catch
{
    // ignored
}

if (autoLoginAsync?.State != true)
    while (true)
    {
        Clear();

        WriteLine("CHMLFRP 登录");
        WriteLine("[1] 用户名密码登录");
        WriteLine("[2] 用户Token登录");
        var readnum = ReadKey(true);
        DataResponse<UserData>? dataResponse = null;
        switch (readnum.Key)
        {
            case ConsoleKey.D1:
            {
                Write("用户名: ");
                var userName = ReadLine();

                Write("密码: ");
                var password = ReadLine();
                try
                {
                    dataResponse = await client.LoginAsync(userName, password);
                }
                catch (Exception e)
                {
                    WriteLine(e.Message);
                }

                break;
            }
            case ConsoleKey.D2:
            {
                Write("用户Token: ");
                var usertoken = ReadLine();

                try
                {
                    dataResponse = await client.LoginByTokenAsync(usertoken!);
                }
                catch (Exception e)
                {
                    WriteLine(e.Message);
                }

                break;
            }
            default:
                continue;
        }

        WriteLine(dataResponse?.Message);

        if (dataResponse?.State == true)
            break;

        await Task.Delay(TimeSpan.FromSeconds(2));
    }

WriteLine("Hello World!");