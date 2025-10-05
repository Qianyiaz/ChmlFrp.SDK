using ConsoleJson;

using var client = new HttpClient();
var getForecast = await client.GetFromJsonAsync($"https://cf-v2.uapis.cn/login?username={Console.ReadLine()}&password={Console.ReadLine()}",GetForecastContext.Default.GetForecast);
Console.WriteLine(getForecast.Message);