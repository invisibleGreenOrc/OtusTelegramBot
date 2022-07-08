using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.InMemoryData.Repositories;
using OtusTelegramBot.Presentation;
using OtusTelegramBot.Services.Abstractions;
using OtusTelegramBot.Services;
using Telegram.Bot.Types;
using OtusTelegramBot.Presentation.Model;
using Telegram.Bot.Types.ReplyMarkups;

namespace OtusTelegramBot
{
    public class CommandExecutor
    {
        private Func<long, string, IReplyMarkup?, Task<Message>> _sendMessage;
        private Func<string, Task> _sendCallbackQueryAnswer;

        private Dictionary<long, string> _userStates = new();
        private Dictionary<long, string> _userNames = new();


        private IRolesRepository _roleRepository;
        private IUsersRepository _usersRepository;
        private IDisciplinesRepository _disciplinesRepository;
        private ILessonsRepository _lessonsRepository;

        private IUsersService _userService;
        private ILessonsService _lessonsService;

        private UsersController _usersController;
        private LessonsController _lessonsController;

        public CommandExecutor(Func<long, string, IReplyMarkup?, Task<Message>> sendMessage, Func<string, Task> sendCallbackQueryAnswer)
        {
            _sendMessage = sendMessage;
            _sendCallbackQueryAnswer = sendCallbackQueryAnswer;

            _roleRepository = new RolesRepository();
            _usersRepository = new UsersRepository(_roleRepository);
            _disciplinesRepository = new DisciplinesRepository();
            _lessonsRepository = new LessonsRepository(_disciplinesRepository, _usersRepository);

            _userService = new UsersService(_usersRepository, _roleRepository);
            _lessonsService = new LessonsService(_lessonsRepository, _disciplinesRepository);

            _usersController = new UsersController(_userService);
            _lessonsController = new LessonsController(_lessonsService, _userService);
        }

        public void ProcessCallbackQuery(long userId, string queryId, string? data)
        {
            if (_userStates.TryGetValue(userId, out var state1) && state1 == "/start.RoleChoice")
            {
                var newUser = new UserForCreatingVM
                {
                    UserId = userId,
                    Name = _userNames[userId],
                    RoleId = int.Parse(data)
                };

                _usersController.AddUser(newUser);
                _sendMessage(userId, $"Профиль успешно создан! Что будем делать дальше?", null);

                _userStates.Remove(userId);
                _userNames.Remove(userId);
            }

            _sendCallbackQueryAnswer(queryId);
        }

        public void ExecuteCommand(string command, long userId, string userName, long chatId)
        {
            if (command == "/start")
            {
                if (_usersController.IsUserExists(userId))
                {
                    _sendMessage(chatId, $"Привет, {_usersController.GetUser(userId).Name}, что будем делать?", null);
                }
                else
                {
                    _userStates[userId] = "/start.NameChoice";

                    _sendMessage(chatId, $"Добро пожаловать!" +
                        $"\nКак тебя зовут?", null);
                }
            }
            else
            {
                if (command == "/sign_up_for_drill")
                {
                    var lessons = GetFutureLessons();

                    _sendMessage(chatId, $"Выбери номер\n {lessons}", null);

                    _userStates[userId] = "viewlist";
                }
                else if (_userStates.TryGetValue(userId, out var state) && state == "/start.NameChoice")
                {
                    _userNames[userId] = command.Trim();
                    _userStates[userId] = "/start.RoleChoice";

                    var roles = _userService.GetAllRoles();
                    var buttons = roles.Select((role, serialNumber) => InlineKeyboardButton.WithCallbackData($"{serialNumber + 1}. {role.Name}", role.Id.ToString()));

                    InlineKeyboardMarkup inlineKeyboard = new(buttons);

                    _sendMessage(chatId, $"Выбери свою роль", inlineKeyboard);
                }
                else if (_userStates.TryGetValue(userId, out var state2) && state2 == "viewlist")
                {
                    if (int.TryParse(command, out int index))
                    {
                        _sendMessage(chatId, $"Вы выбрали {index}", null);

                        _userStates[userId] = string.Empty;
                    }
                    else
                    {
                        var lessons = GetFutureLessons();

                        _sendMessage(chatId, $"Неверный номер!\n Выбери номер\n {lessons}", null);
                    }
                }
            }
        }

        private string GetFutureLessons()
        {
            var lessons = _lessonsController.GetFutureLessons()
                            .Select(lesson => $"{lesson.Date} {lesson.DisciplineName}, сложность - {lesson.Difficulty}, тренер - {lesson.TrainerDesc}").ToList();

            var lessonsDescription = string.Join('\n', lessons);

            if (string.IsNullOrEmpty(lessonsDescription))
            {
                return "Нет тренировок";
            }

            return lessonsDescription;
        }
    }
}
