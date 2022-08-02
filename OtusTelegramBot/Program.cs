using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace OtusTelegramBot
{
    public class Program
    {
        private static TelegramBotClient _botClient;

        private static CommandExecutor _commandExecutor;

        private static void Main(string[] args)
        {
            _botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("tgToken", EnvironmentVariableTarget.User));

            _commandExecutor = new CommandExecutor(SendMessage, SendCallbackQueryAnswer);

            StartTelegraBot();

            Console.ReadLine();
        }

        private static async void StartTelegraBot()
        {
            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await _botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.CallbackQuery is { } query)
            {
                _commandExecutor.ProcessCallbackQuery(query.Message.Chat.Id, query.From.Id, query.Id, query.Data);
                //HandleCallbackQuery
            }

            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
            {
                return;
            }
            // Only process text messages
            if (message.Text is not { } messageText)
            {
                return;
            }

            var chatId = message.Chat.Id;
            var userId = message.From.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}. -- {message.From.Username}");


            _commandExecutor.ExecuteCommand(message.Text, userId, chatId);
            //HandleMessage
        }

        private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task<Message> SendMessage(long chatId, string messageText, IReplyMarkup? replyMarkup = null)
        {
            Message sentMessage = await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: messageText,
                    replyMarkup: replyMarkup,
                    cancellationToken: default);

            return sentMessage;
        }

        public static async Task SendCallbackQueryAnswer(string queryId)
        {
            await _botClient.AnswerCallbackQueryAsync("queryId");
        }
    }
}