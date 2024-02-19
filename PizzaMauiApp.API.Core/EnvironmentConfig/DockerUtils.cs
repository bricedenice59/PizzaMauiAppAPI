using System.Text.Json;
using Microsoft.Extensions.FileProviders;

namespace PizzaMauiApp.API.Core.EnvironmentConfig;

public class DockerUtils
{
    public static T? GetSecrets<T>(string key)
    {
        const string DOCKER_SECRET_PATH = "/run/secrets/";
        if (!Directory.Exists(DOCKER_SECRET_PATH))
        {
            Console.WriteLine($"Directory {DOCKER_SECRET_PATH} does not exist");
            return default;
        }
        
        try
        {
            var provider = new PhysicalFileProvider(DOCKER_SECRET_PATH);
            var fileInfo = provider.GetFileInfo(key);
            if (!fileInfo.Exists)
            {
                Console.WriteLine($"File {fileInfo.Name} does not exist");
                return default;
            }
        
            using var stream = fileInfo.CreateReadStream();
            using var streamReader = new StreamReader(stream);
            return JsonSerializer.Deserialize<T>(streamReader.ReadToEnd());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return default;
    }
}