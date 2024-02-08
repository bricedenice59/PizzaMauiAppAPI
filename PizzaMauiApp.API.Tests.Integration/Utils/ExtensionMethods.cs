using System.Net.Http.Json;

namespace PizzaMauiApp.API.Tests.Integration.Utils;

public static class ExtensionMethods
{
    public static Task<T?> GetAndDeserialize<T>(this HttpClient client, string requestUri)
    {
        return client.GetFromJsonAsync<T>(requestUri);
    }
}
