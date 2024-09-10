using System.Text;
using System.Text.Json;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Test;

public static class HttpClientExtensions
{
    public static async Task<(HttpResponseMessage, T)> Post<T>(this HttpClient httpClient,
        string uri,
        object data) =>
        await Deserialize<T>(await httpClient.PostAsync(new Uri(uri, UriKind.Relative),
            new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json")));

    private static async Task<(HttpResponseMessage, T)> Deserialize<T>(HttpResponseMessage responseMessage) =>
        (responseMessage,
            (await responseMessage.Content.ReadAsStringAsync()).Deserialize<T>() ??
            throw new Exception($"Failed to deserialize {nameof(T)}"));

    public static async Task<(HttpResponseMessage, T)> PostManager<T>(this HttpClient httpClient) =>
        await Post<T>(httpClient,
            "/api/managers",
            new CreateManagerRequest
                { Name = new ManagerName("Jakob"), Email = new Email("jakob@jakobsson.com") });
}