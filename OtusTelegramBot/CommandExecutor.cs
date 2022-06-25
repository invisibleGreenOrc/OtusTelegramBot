using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.InMemoryData.Repositories;
using OtusTelegramBot.Presentation;
using OtusTelegramBot.Services.Abstractions;
using OtusTelegramBot.Services;
using Telegram.Bot.Types;

namespace OtusTelegramBot
{
    public class CommandExecutor
    {
        private Func<long, string, Task<Message>> _sendMessage;
        private Dictionary<long, string> _userStates = new();

        private IRolesRepository _roleRepository;
        private IUsersRepository _usersRepository;
        private IDisciplinesRepository _disciplinesRepository;
        private ILessonsRepository _lessonsRepository;

        private IUsersService _userService;
        private ILessonsService _lessonsService;

        private UsersController _usersController;
        private LessonsController _lessonsController;

        public CommandExecutor(Func<long, string, Task<Message>> sendMessage)
        {
            _sendMessage = sendMessage;

        _roleRepository = new RolesRepository();
        _usersRepository = new UsersRepository(_roleRepository);
        _disciplinesRepository = new DisciplinesRepository();
        _lessonsRepository = new LessonsRepository(_disciplinesRepository, _usersRepository);

        _userService = new UsersService(_usersRepository, _roleRepository);
        _lessonsService = new LessonsService(_lessonsRepository, _disciplinesRepository);

        _usersController = new UsersController(_userService);
        _lessonsController = new LessonsController(_lessonsService, _userService);
    }

        public void ExecuteCommand(string command, (long userId, string userName) userInfo, long chatId)
        {
            if (command == "/start")
            {
                if (_userStates.ContainsKey(userInfo.userId))
                {
                    _sendMessage(chatId, $"Привет, {userInfo.userName}, что будем делать?");
                }
                else
                {
                    _userStates.Add(userInfo.userId, string.Empty);

                    _sendMessage(chatId, $"Добро пожаловать, {userInfo.userName}!");
                }

            }
            else
            {
                if (!_userStates.ContainsKey(userInfo.userId))
                {
                    _sendMessage(chatId, "Вы не зареганы, введите /start");
                }
                else if (command == "/sign_up_for_drill")
                {
                    var lessons = GetFutureLessons();

                    _sendMessage(chatId, $"Выбери номер\n {lessons}");

                    _userStates[userInfo.userId] = "viewlist";
                }
                else if (_userStates[userInfo.userId] == "viewlist")
                {
                    if (int.TryParse(command, out int index))
                    {
                        _sendMessage(chatId, $"Вы выбрали {index}");

                        _userStates[userInfo.userId] = string.Empty;
                    }
                    else
                    {
                        var lessons = GetFutureLessons();

                        _sendMessage(chatId, $"Неверный номер!\n Выбери номер\n {lessons}");
                    }
                }
            }

            //Console.WriteLine($"Received a '{command}' message in chat {chatId}. -- {userInfo.userName}");


            foreach (var state in _userStates)
            {
                Console.WriteLine($"{ state.Key } - { state.Value }");
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
