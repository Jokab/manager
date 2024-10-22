using System.Text;
using System.Text.Json;
using ManagerGame.Api;

namespace ManagerGame.Test.Api;

public static class HttpClientExtensions
{
    public static async Task<(HttpResponseMessage, T?)> Post<T>(this HttpClient httpClient,
        string uri,
        object data)
    {
        var httpResponse = await httpClient.PostAsync(new Uri(uri, UriKind.Relative),
            new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json"));
        return httpResponse switch
        {
            { IsSuccessStatusCode: true } response => await Deserialize<T>(response),
            { } response => (response, default)
        };
    }

    private static async Task<(HttpResponseMessage, T)> Deserialize<T>(HttpResponseMessage responseMessage) =>
        (responseMessage,
            (await responseMessage.Content.ReadAsStringAsync()).Deserialize<T>() ??
            throw new Exception($"Failed to deserialize {nameof(T)}"));

    public static async Task<(HttpResponseMessage, T?)> PostManager<T>(this HttpClient httpClient) =>
        await Post<T>(httpClient,
            "/api/managers",
            new CreateManagerRequest
                { Name = "Jakob", Email = $"jakob{Guid.NewGuid()}@jakobsson.com" });

    public static async Task<(HttpResponseMessage, T?)> Get<T>(this HttpClient httpClient,
        string uri)
    {
        return await httpClient.GetAsync(new Uri(uri, UriKind.Relative)) switch
        {
            { IsSuccessStatusCode: true } response => await Deserialize<T>(response),
            { } response => (response, default)
        };
    }
}
