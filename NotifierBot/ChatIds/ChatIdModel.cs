using Realms;

namespace NotifierBot.ChatIds;

public class ChatId : RealmObject
{
    [PrimaryKey]
    public long Id { get; set; }
}