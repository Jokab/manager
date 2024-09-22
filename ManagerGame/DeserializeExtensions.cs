using System.Text.Json;

namespace ManagerGame;

public static class DeserializeExtensions
{
    private static readonly JsonSerializerOptions? DefaultSerializerSettings = new()
    {
        PropertyNameCaseInsensitive = true,
        IncludeFields = true,
        ReferenceHandler = null
    };

    public static T? Deserialize<T>(this string json) => JsonSerializer.Deserialize<T>(json, DefaultSerializerSettings);
}
