using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;

namespace ChmlFrp.SDK;

public static class HttpClientExtensions
{
    /// <summary>
    ///     自定义HttpClient,没配置也没关系,自动配置.
    /// </summary>
    public static HttpClient MainClient;

    /// <summary>
    ///     对HttpClient的扩展Json解析,支持Aot.
    /// </summary>
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