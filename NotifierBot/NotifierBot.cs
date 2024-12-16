using NotifierBot.ChatIds;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace NotifierBot;

public class NotifierBot(string botToken)
{
    private readonly TelegramBotClient _botClient = new(botToken);
    private readonly ChatIdsRepository _chatIdsRepository = new();

    public void Start()
    {
        Log.Information("Starting Telegram bot...");

        _botClient.StartReceiving(
            HandleUpdate,
            HandleError,
            new ReceiverOptions()
        );

        Log.Information("Telegram bot is running.");
    }

    private async Task HandleUpdate(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message &&
            !string.IsNullOrEmpty(update.Message?.Text))
        {
            var chatId = update.Message.Chat.Id;

            _chatIdsRepository.AddChatId(chatId);
            await botClient.SendMessage(
                chatId,
                "Hey! You'll receive notifications if no files are created or if there's an extended period without file creation.",
                cancellationToken: cancellationToken);
        }
    }

    private Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiException => $"Telegram API Error:\n[{apiException.ErrorCode}]\n{apiException.Message}",
            _ => exception.ToString()
        };

        Log.Error(errorMessage);
        return Task.CompletedTask;
    }

    public async Task NotifyAllUsers(string message)
    {
        foreach (var chatId in _chatIdsRepository.GetAllChatIds())
        {
            try
            {
                await _botClient.SendMessage(chatId, message);
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == TelegramErrorCodes.Forbidden || ex.ErrorCode == TelegramErrorCodes.BadRequest)
            {
                Log.Information($"Chat ID {chatId} is invalid. Removing...");
                _chatIdsRepository.RemoveChatId(chatId);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to send message to {chatId}: {ex.Message}");
            }
        }
    }
}

public static class TelegramErrorCodes
{
    public const int BadRequest = 400;
    public const int Forbidden = 403;
}