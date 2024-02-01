namespace PizzaMauiApp.API.Helpers.EnvironmentConfig;

public class DbConnectionConfig
{
    public string? Database { get; set; }
    public string? Host { get; set; }
    public string? Port { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }

    public DbConnectionConfig()
    {
        
    }
    
    //environment variables in DEBUG are stored on my local machine using: dotnet user-secrets
    //Db properties are stored as following "Db"+"{dbFunctionalityName}" + class property
    //ex: DbStoreName - DbIdentityPassword...
    public DbConnectionConfig(ConfigurationManager configuration, string dbFunctionalityName)
    {
        Database = configuration[$"Db{dbFunctionalityName}Name"];
        Host = configuration[$"Db{dbFunctionalityName}Host"];
        Port = configuration[$"Db{dbFunctionalityName}Port"];
        User = configuration[$"Db{dbFunctionalityName}User"];
        Password = configuration[$"Db{dbFunctionalityName}Password"];
    }
    
    //environment variables in RELEASE are stored using docker secret functionality
    public DbConnectionConfig(string dockerSecretKey)
    {
        var dbConnectionConfig = DockerUtils.GetSecrets<DbConnectionConfig>(dockerSecretKey);
        if (dbConnectionConfig == null) return;

        Database = dbConnectionConfig.Password;
        Host = dbConnectionConfig.Host;
        Port = dbConnectionConfig.Port;
        User = dbConnectionConfig.User;
        Password = dbConnectionConfig.Password;
    }

    public override string ToString()
    {
        return $"Database={Database};Username={User};Password={Password};Host={Host};Port={Port}";
    }
}