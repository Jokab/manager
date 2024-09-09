using System.Text;
using System.Text.Json;
using ManagerGame.Core.Commands;
using ManagerGame.Core.Domain;

namespace ManagerGame.Test;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> Post(this HttpClient httpClient,
        string uri,
        object data)
    {
        return await httpClient.PostAsync(new Uri(uri, UriKind.Relative),
            new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json"));
    }

    public static async Task<HttpResponseMessage> PostManager(this HttpClient httpClient)
    {
        return await Post(httpClient,
            "/api/managers",
            new CreateManagerRequest
                { Name = new ManagerName("Jakob"), Email = new Email("jakob@jakobsson.com") });
    }
}