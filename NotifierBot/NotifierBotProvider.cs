namespace RecordingsTracker.NotifierBot;

public static class NotifierBotProvider
{
    private static global::NotifierBot.NotifierBot? _notifierBot;
    public static global::NotifierBot.NotifierBot NotifierBot => _notifierBot!;

    public static global::NotifierBot.NotifierBot InitiateBot(string botToken)
    {
        _notifierBot = new global::NotifierBot.NotifierBot(botToken);

        _notifierBot.Start();

        return _notifierBot;
    }

}