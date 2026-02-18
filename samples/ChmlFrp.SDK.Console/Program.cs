using ChmlFrp.SDK.Content;
using ChmlFrp.SDK.Models;
using ChmlFrp.SDK.Service;
using static System.Console;

var client = new ChmlFrpClient();

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
        Write("用户名: ");
        var userName = ReadLine();

        Write("密码: ");
        var password = ReadLine();

        DataResponse<UserData>? dataResponse = null;
        try
        {
            dataResponse = await client.LoginAsync(userName, password); // 登录
        }
        catch (Exception e)
        {
            WriteLine(e.Message);
        }

        WriteLine(dataResponse?.Message);
        
        if (dataResponse?.State == true)
            break;

        await Task.Delay(TimeSpan.FromSeconds(3));
    }

WriteLine("Hello World!");