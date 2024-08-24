using System.Text.Json;

namespace ManagerGame;

public static class DeserializeExtensions
{
	private static readonly JsonSerializerOptions? DefaultSerializerSettings = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public static T? Deserialize<T>(this string json)
	{
		return JsonSerializer.Deserialize<T>(json, DefaultSerializerSettings);
	}
}
