using Core.Configuration;
using RecordingsTracker.NotifierBot;
using Serilog;

namespace RecordingsTracker;
internal class Program
{
    static void Main(string[] args)
    {
        ConfigureLogging();

        NotifierBotProvider.InitiateBot(ConfigurationProvider.Configuration.NotifierBotToken);

        var listener = new FolderListener();
        listener.Listen();

        Console.ReadLine();
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
           .WriteTo.Console()
           .WriteTo.File(ConfigurationProvider.Configuration.LogPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
           .CreateLogger();
    }
}
