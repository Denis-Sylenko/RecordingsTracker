
using Core.Configuration;
using RecordingsTracker.NotifierBot;
using Serilog;

namespace RecordingsTracker;

public class FileCreationActivityMonitor
{
    private CancellationTokenSource? _lastTimeFileCreatedCt;

    private static FileCreationActivityMonitor? _instance;
    public static FileCreationActivityMonitor Instance => _instance ??= new FileCreationActivityMonitor();

    public void ResetTimer()
    {
        _lastTimeFileCreatedCt?.Cancel();
        _lastTimeFileCreatedCt?.Dispose();
        _lastTimeFileCreatedCt = new CancellationTokenSource();

        Task.Run(() => StartTimer(_lastTimeFileCreatedCt.Token));
    }

    private async Task StartTimer(CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(ConfigurationProvider.Configuration.FileCreationTimeGapInSeconds), token);

        if (!token.IsCancellationRequested)
        {
            var warningMessage = $"No files have been created in the last {ConfigurationProvider.Configuration.FileCreationTimeGapInSeconds} seconds.";
            await NotifierBotProvider.NotifierBot.NotifyAllUsers(warningMessage);
            Log.Warning(warningMessage);
            ResetTimer();
        };
    }

    private FileCreationActivityMonitor() { }
}
