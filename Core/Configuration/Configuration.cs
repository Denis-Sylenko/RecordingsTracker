namespace Core.Configuration;

public record Configuration
{
    public string[] PathsToRecordingFolders { get; init; }
    public bool CheckFoldersRecursively { get; init; }
    public int FileCreationTimeGapInSeconds { get; init; }
    public string DbPath { get; init; }
    public string LogPath { get; init; }
    public string NotifierBotToken { get; init; }
}
