using System.Text;
using System.Text.Json;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Test.Api;

public static class HttpClientExtensions
{
    public static async Task<(HttpResponseMessage, T?)> Post<T>(this HttpClient httpClient,
        string uri,
        object data)
    {
        return await httpClient.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json")) switch
            {
                { IsSuccessStatusCode: true } response => await Deserialize<T>(response),
                { } response => (response, default)
            };
    }

    private static async Task<(HttpResponseMessage, T)> Deserialize<T>(HttpResponseMessage responseMessage)
    {
        return (responseMessage,
            (await responseMessage.Content.ReadAsStringAsync()).Deserialize<T>() ??
            throw new Exception($"Failed to deserialize {nameof(T)}"));
    }

    public static async Task<(HttpResponseMessage, T?)> PostManager<T>(this HttpClient httpClient)
    {
        return await Post<T>(httpClient,
            "/api/managers",
            new CreateManagerRequest
                { Name = new ManagerName("Jakob"), Email = new Email($"jakob{Guid.NewGuid()}@jakobsson.com") });
    }

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
