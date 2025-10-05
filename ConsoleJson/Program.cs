using System.Text.Json;
using ConsoleJson;
using ConsoleJson.Forecast;
using static System.Console;

using var client = new HttpClient();

while (true)
{
    UserForecast forecast;
    try
    {
        forecast = await client.GetFromJsonAsync(
            $"https://cf-v2.uapis.cn/login?username={ReadLine()}&password={ReadLine()}",
            UserForecastContext.Default.UserForecast
        );
    }
    catch (Exception ex) when (ex is HttpRequestException or JsonException)
    {
        WriteLine(ex.Message);
        ReadKey();
        continue;
    }

    WriteLine(forecast.Message);
    if (forecast.State)
    {
        var i = 1;
        foreach (var tunnel in await forecast.Data.GetTunnelList(client))
        {
            WriteLine($"{i}. {tunnel.Name}");
            i++;
        }

        ReadKey();
        return;
    }
}