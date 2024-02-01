namespace PizzaMauiApp.API.Helpers.EnvironmentConfig;

public class RedisConnectionConfig
{
    public string? Host { get; set; }
    public string? Port { get; set; }
    public bool IsSSL { get; set; }
    public string? Password { get; set; }

    public RedisConnectionConfig()
    {
        
    }
}