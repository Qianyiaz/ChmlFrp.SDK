using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;

namespace ConsoleJson;

public static class HttpClientExtensions
{
    public static async Task<T> GetFromJsonAsync<T>
    (
        this HttpClient client,
        string requestUri,
        JsonTypeInfo<T> jsonTypeInfo
    )
    {
        using var response = await client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(jsonTypeInfo);
    }
}