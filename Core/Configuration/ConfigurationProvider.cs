using System.Text.Json;

namespace Core.Configuration;

public static class ConfigurationProvider
{
    private static Configuration? _configuration;
    public static Configuration Configuration => _configuration ??= GetConfiguration();

    private static Configuration GetConfiguration()
    {
        var configJson = File.ReadAllText(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "configuration.json"));

        return JsonSerializer.Deserialize<Configuration>(configJson)!;
    }

}