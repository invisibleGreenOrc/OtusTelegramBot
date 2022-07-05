using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.InMemoryData.Repositories;
using OtusTelegramBot.Presentation;
using OtusTelegramBot.Services.Abstractions;
using OtusTelegramBot.Services;
using Telegram.Bot.Types;
using System.Text;
using OtusTelegramBot.Presentation.Model;

namespace OtusTelegramBot
{
    public class CommandExecutor
    {
        private Func<long, string, Task<Message>> _sendMessage;
        private Dictionary<long, string> _userStates = new();
        private Dictionary<long, string> _userNames = new();
        private Dictionary<long, int[]> _allowedRolesForUser = new();


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

        public void ExecuteCommand(string command, long userId, string userName, long chatId)
        {
            if (command == "/start")
            {
                if (_usersController.IsUserExists(userId))
                {
                    _sendMessage(chatId, $"Привет, {_usersController.GetUser(userId).Name}, что будем делать?");
                }
                else
                {
                    _userStates[userId] = "/start.NameChoice";

                    _sendMessage(chatId, $"Добро пожаловать!" +
                        $"\nКак тебя зовут?");
                }
            }
            else
            {
                if (command == "/sign_up_for_drill")
                {
                    var lessons = GetFutureLessons();

                    _sendMessage(chatId, $"Выбери номер\n {lessons}");

                    _userStates[userId] = "viewlist";
                }
                else if (_userStates.TryGetValue(userId, out var state) && state == "/start.NameChoice")
                {
                    _userNames[userId] = command.Trim();

                    _userStates[userId] = "/start.RoleChoice";

                    var roles = _userService.GetAllRoles();
                    var rolesList = new StringBuilder();

                    // Привязываться к Id вряд ли хорошая идея
                    foreach (var role in roles)
                    {
                        rolesList.Append($"{role.Id}. {role.Name}\n");
                    }

                    _sendMessage(chatId, $"Выбери номер роли" +
                        $"\n{rolesList}");

                    _allowedRolesForUser[userId] = roles.Select(role => role.Id).ToArray();
                }
                else if (_userStates.TryGetValue(userId, out var state1) && state1 == "/start.RoleChoice")
                {
                    if (int.TryParse(command, out int index) && _allowedRolesForUser.TryGetValue(userId, out var rolesIds) && rolesIds.Contains(index))
                    {
                        _sendMessage(chatId, $"Вы выбрали {index}");

                        _userStates[userId] = string.Empty;

                        var newUser = new UserForCreatingVM
                        {
                            UserId = userId,
                            Name = _userNames[userId],
                            RoleId = index
                        };

                        _usersController.AddUser(newUser);

                        _sendMessage(chatId, $"Профиль успешно создан! Что будем делать дальше?");


                        _allowedRolesForUser.Remove(userId);
                        _userStates.Remove(userId);
                        _userNames.Remove(userId);
                    }
                    else
                    {
                        var roles = _userService.GetAllRoles();
                        var rolesList = new StringBuilder();

                        // Привязываться к Id вряд ли хорошая идея
                        foreach (var role in roles)
                        {
                            rolesList.Append($"{role.Id}. {role.Name}\n");
                        }

                        _sendMessage(chatId, $"Неверный номер! Выбери номер роли" +
                            $"\n{rolesList}");

                        _allowedRolesForUser[userId] = roles.Select(role => role.Id).ToArray();
                    }
                }
                else if (_userStates.TryGetValue(userId, out var state2) && state2 == "viewlist")
                {
                    if (int.TryParse(command, out int index))
                    {
                        _sendMessage(chatId, $"Вы выбрали {index}");

                        _userStates[userId] = string.Empty;
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
