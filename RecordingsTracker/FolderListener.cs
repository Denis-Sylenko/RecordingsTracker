using Core.Configuration;
using RecordingsTracker.NotifierBot;
using Serilog;

namespace RecordingsTracker;


public sealed class FolderListener() : IDisposable
{
    private const string StreamFileExtension = ".stream";
    private const string DeflateFileExtension = ".deflate";
    private List<FileSystemWatcher> _fileSystemWatchers = [];

    public void Listen()
    {
        _fileSystemWatchers = ConfigurationProvider.Configuration.PathsToRecordingFolders.Select(CreateWatcher).ToList();
        FileCreationActivityMonitor.Instance.ResetTimer();
    }

    public void Dispose()
    {
        foreach (var watcher in _fileSystemWatchers)
        {
            watcher.Created -= OnFileCreated;
            watcher.Deleted -= OnFileDeleted;
            watcher.Dispose();
        }

        _fileSystemWatchers.Clear();
    }

    private FileSystemWatcher CreateWatcher(string path)
    {
        var watcher = new FileSystemWatcher(path)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            IncludeSubdirectories = ConfigurationProvider.Configuration.CheckFoldersRecursively,
            EnableRaisingEvents = true,
        };

        watcher.Created += OnFileCreated;
        watcher.Deleted += OnFileDeleted;

        return watcher;
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        Log.Information($"The file {e.FullPath} has been created.");
        if (!string.IsNullOrEmpty(e.Name) && e.Name.EndsWith(StreamFileExtension, StringComparison.InvariantCultureIgnoreCase))
        {
            FileCreationActivityMonitor.Instance.ResetTimer();
        }
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        Log.Information($"File {e.FullPath} has been deleted.");
        if (!string.IsNullOrEmpty(e.Name) && e.Name.EndsWith(StreamFileExtension, StringComparison.InvariantCultureIgnoreCase))
        {
            HandleStreamFileDeleted(e);
        }
    }

    private async Task HandleStreamFileDeleted(FileSystemEventArgs e)
    {
        // Wait some time for the case there is a delay between .stream
        // file deletion and changing size of the .stream.deflate file
        await Task.Delay(1000);

        var deflateFilePath = $"{e.FullPath}{DeflateFileExtension}";

        var fileInfo = new FileInfo(deflateFilePath);
        if (fileInfo.Exists && fileInfo.Length == 0)
        {
            var warningMessage = $"File {deflateFilePath} is empty after compression.";
            await NotifierBotProvider.NotifierBot.NotifyAllUsers(warningMessage);
            Log.Warning(warningMessage);
        }
    }
}