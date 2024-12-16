using Core.Configuration;
using Realms;
using Serilog;

namespace NotifierBot.ChatIds;

public sealed class ChatIdsRepository
{
    private readonly RealmConfiguration _config;

    public ChatIdsRepository()
    {
        _config = new RealmConfiguration(ConfigurationProvider.Configuration.DbPath);
    }

    public void AddChatId(long chatId)
    {
        try
        {
            using var realm = Realm.GetInstance(_config);
            realm.Write(() => realm.Add(new ChatId { Id = chatId }));
        }
        catch (Exception ex)
        {
            Log.Error($"Error adding ChatId: {ex.Message}");
        }
    }

    public void RemoveChatId(long chatId)
    {
        try
        {
            using var realm = Realm.GetInstance(_config);
            var chatIdObj = realm.Find<ChatId>(chatId);
            if (chatIdObj != null)
            {
                realm.Write(() => realm.Remove(chatIdObj));
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error removing ChatId: {ex.Message}");
        }
    }

    public List<long> GetAllChatIds()
    {
        try
        {
            using var realm = Realm.GetInstance(_config);
            return realm.All<ChatId>().ToList().Select(c => c.Id).ToList();
        }
        catch (Exception ex)
        {
            Log.Error($"Error retrieving ChatIds: {ex.Message}");
            return [];
        }
    }
}
