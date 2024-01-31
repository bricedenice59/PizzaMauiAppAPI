namespace PizzaMauiApp.API.Helpers;

public class RedisConnection
{
    public string? Host { get; set; }
    public string? Port { get; set; }
    public bool IsSSL { get; set; }
    public string? Password { get; set; }
}