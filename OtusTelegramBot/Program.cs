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

        static void Main(string[] args)
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
                _commandExecutor.ProcessCallbackQuery(query.From.Id, query.Id, query.Data);
            }

            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;
            var userId = message.From.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}. -- {message.From.Username}");

            
            _commandExecutor.ExecuteCommand(message.Text, userId, chatId);
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





        // Что-то не то
        //public static UserVM GetOrCreateUser(string telegramId)
        //{
        //    var user = _usersController.GetUser(telegramId);

        //    if (user is null)
        //    {
        //        CreateNewUser(telegramId);
        //    }

        //    user = _usersController.GetUser(telegramId);
        //    GreetUser(user);

        //    return user;
        //}

        //private static void CreateNewUser(string telegramId)
        //{
        //    var userToCreate = GetUserData();
        //    userToCreate.TelegramId = telegramId;

        //    _usersController.AddUser(userToCreate);

        //    _userInterface.PostOutput($"Новый пользователь для {userToCreate.TelegramId} создан!");
        //}

        //private static UserForCreatingVM GetUserData()
        //{
        //    var user = new UserForCreatingVM();

        //    _userInterface.PostOutput("Введите ваше имя");
        //    user.Name = _userInterface.GetInput();

        //    var roles = _userService.GetAllRoles();
        //    var rolesList = new StringBuilder();

        //    // Привязываться к Id вряд ли хорошая идея
        //    foreach (var role in roles)
        //    {
        //        rolesList.Append($"{role.Id}. {role.Name}\n");
        //    }

        //    var roleId = 0;

        //    do
        //    {
        //        _userInterface.PostOutput($"Выберите роль. Выберите номер роли из списка:\n{rolesList}");
        //    }
        //    while (!int.TryParse(_userInterface.GetInput(), out roleId) || !roles.Exists(role => role.Id == roleId));

        //    user.RoleId = roleId;

        //    return user;
        //}
    }
}