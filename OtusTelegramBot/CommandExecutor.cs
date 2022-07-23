using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.InMemoryData.Repositories;
using OtusTelegramBot.Presentation;
using OtusTelegramBot.Services.Abstractions;
using OtusTelegramBot.Services;
using Telegram.Bot.Types;
using OtusTelegramBot.Presentation.Model;
using Telegram.Bot.Types.ReplyMarkups;
using OtusTelegramBot.InMemoryData;
using OtusTelegramBot.Domain.Entities;
using System.Text;

namespace OtusTelegramBot
{
    public class CommandExecutor
    {
        private Func<long, string, IReplyMarkup?, Task<Message>> _sendMessage;
        private Func<string, Task> _sendCallbackQueryAnswer;

        private Dictionary<long, string> _userStates = new();
        private Dictionary<long, string> _userNames = new();
        private Dictionary<long, (int disciplineId, int levelId, DateOnly date, TimeOnly time)> _userDrill = new();


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
            _lessonsService = new LessonsService(_lessonsRepository, _disciplinesRepository, _userService);

            _usersController = new UsersController(_userService);
            _lessonsController = new LessonsController(_lessonsService, _userService);
        }

        public void ProcessCallbackQuery(long chatId, long userId, string queryId, string? data)
        {
            string? state = string.Empty;

            if (_userStates.TryGetValue(userId, out state) && state == "/start.ChooseRole")
            {
                var newUser = new UserForCreatingVM
                {
                    UserId = userId,
                    Name = _userNames[userId],
                    RoleId = int.Parse(data)
                };

                _usersController.AddUser(newUser);
                _sendMessage(chatId, $"{newUser.Name}, профиль успешно создан! Что будем делать дальше?", null);

                _userStates.Remove(userId);
                _userNames.Remove(userId);
            }
            else if (_userStates.TryGetValue(userId, out state) && state == "/create_drill.ChooseDiscipline")
            {
                _userDrill[userId] = (disciplineId: int.Parse(data), 0, DateOnly.MinValue, TimeOnly.MinValue);

                _userStates[userId] = "/create_drill.ChooseLevel";

                var levels = _lessonsController.GetDifficultyLevels();
                var buttons = levels.Select((level, index) => new[]
                                {
                                        InlineKeyboardButton.WithCallbackData($"{index + 1}. {level.Name}", level.Id.ToString())
                                    });

                var inlineKeyboard = new InlineKeyboardMarkup(buttons);

                _sendMessage(chatId, "Выбери уровень", inlineKeyboard);
            }
            else if (_userStates.TryGetValue(userId, out state) && state == "/create_drill.ChooseLevel")
            {
                var drillData = _userDrill[userId];
                drillData.levelId = int.Parse(data);
                _userDrill[userId] = drillData;

                _userStates[userId] = "/create_drill.EnterDate";

                _sendMessage(chatId, "Укажи дату занятия в формате dd.mm.yyyy", null);
            }
            else if (_userStates.TryGetValue(userId, out state) && state == "/create_drill.ChooseTime")
            {
                var time = data.Trim().Split(':');

                var drillData = _userDrill[userId];
                drillData.time = new TimeOnly(int.Parse(time[0]), int.Parse(time[1]));
                _userDrill[userId] = drillData;

                var newDrill = new LessonForCreatingVM
                {
                    DisciplineId = _userDrill[userId].disciplineId,
                    DifficultyId = _userDrill[userId].levelId,
                    Date = _userDrill[userId].date,
                    Time = _userDrill[userId].time,
                    TrainerId = _usersController.GetUser(userId).Id
                };

                _lessonsController.AddLesson(newDrill);
                _sendMessage(chatId, $"Занятие создано {_userDrill[userId]}. Что будем делать дальше?", null);

                _userStates.Remove(userId);
                _userDrill.Remove(userId);
            }
            else if (_userStates.TryGetValue(userId, out state) && state == "/enroll_to_drill")
            {
                _lessonsController.AddLessonParticipant(int.Parse(data), userId);

                _sendMessage(chatId, $"Вы записаны.", null);
                _userStates.Remove(userId);
            }

            _sendCallbackQueryAnswer(queryId);
        }

        public void ExecuteCommand(string command, long userId, long chatId)
        {
            if (command == "/start")
            {
                if (_usersController.IsUserExists(userId))
                {
                    _sendMessage(chatId, $"Привет, {_usersController.GetUser(userId).Name}, что будем делать?", null);
                }
                else
                {
                    _userStates[userId] = "/start.ChooseName";

                    _sendMessage(chatId, $"Добро пожаловать!" +
                        $"\nКак тебя зовут?", null);
                }
            }
            else
            {
                if (command == "/create_drill")
                {
                    _userStates[userId] = "/create_drill.ChooseDiscipline";

                    var discilpines = _lessonsController.GetAllDisciplines();
                    var buttons = discilpines.Select((discilpine, index) => new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData($"{index + 1}. {discilpine.Name}", discilpine.Id.ToString())
                                    });

                    var inlineKeyboard = new InlineKeyboardMarkup(buttons);

                    _sendMessage(chatId, "Выбери дисциплину", inlineKeyboard);


                }
                else if (_userStates.TryGetValue(userId, out var state) && state == "/start.ChooseName")
                {
                    _userNames[userId] = command.Trim();
                    _userStates[userId] = "/start.ChooseRole";

                    var roles = _usersController.GetAllRoles();
                    var buttons = roles.Select((role, index) => InlineKeyboardButton.WithCallbackData($"{index + 1}. {role.Name}", role.Id.ToString()));

                    var inlineKeyboard = new InlineKeyboardMarkup(buttons);

                    _sendMessage(chatId, $"Выбери свою роль", inlineKeyboard);
                }
                else if (_userStates.TryGetValue(userId, out var state1) && state1 == "/create_drill.EnterDate")
                {
                    var date = command.Trim().Split('.');

                    var drillData = _userDrill[userId];
                    drillData.date = new DateOnly(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));
                    _userDrill[userId] = drillData;

                    _userStates[userId] = "/create_drill.ChooseTime";

                    var buttons = new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "6:00", callbackData: "6:00"),
                            InlineKeyboardButton.WithCallbackData(text: "7:00", callbackData: "7:00"),
                            InlineKeyboardButton.WithCallbackData(text: "8:00", callbackData: "8:00")
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "9:00", callbackData: "9:00"),
                            InlineKeyboardButton.WithCallbackData(text: "10:00", callbackData: "10:00"),
                            InlineKeyboardButton.WithCallbackData(text: "11:00", callbackData: "11:00")
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "12:00", callbackData: "12:00"),
                            InlineKeyboardButton.WithCallbackData(text: "13:00", callbackData: "13:00"),
                            InlineKeyboardButton.WithCallbackData(text: "14:00", callbackData: "14:00")
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "15:00", callbackData: "15:00"),
                            InlineKeyboardButton.WithCallbackData(text: "16:00", callbackData: "16:00"),
                            InlineKeyboardButton.WithCallbackData(text: "17:00", callbackData: "17:00")
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "18:00", callbackData: "18:00"),
                            InlineKeyboardButton.WithCallbackData(text: "19:00", callbackData: "19:00"),
                            InlineKeyboardButton.WithCallbackData(text: "20:00", callbackData: "20:00")
                        }
                    };

                    var inlineKeyboard = new InlineKeyboardMarkup(buttons);

                    _sendMessage(chatId, $"Выбери время", inlineKeyboard);
                }
                else if (command == "/enroll_to_drill")
                {
                    var drills = _lessonsController.GetFutureLessons();
                    var buttons = drills.OrderBy(drill => drill.Date).Select((drill, index) => new[]
                    {
                        InlineKeyboardButton.WithCallbackData($"{index + 1}. {drill.Date} {drill.DisciplineName}, сложность - {drill.Difficulty}," +
                                                                $"тренер - {drill.TrainerDesc}", drill.Id.ToString())
                    });

                    var inlineKeyboard = new InlineKeyboardMarkup(buttons);
                    _sendMessage(chatId, $"Выбери тренировку", inlineKeyboard);

                    _userStates[userId] = "/enroll_to_drill";
                }
                else if (command == "/my_drills")
                {
                    var sortedDrills = _lessonsController.GetFutureLessonsForUser(userId).OrderBy(drill => drill.Date).ToList();

                    if (sortedDrills == null || !sortedDrills.Any())
                    {
                        _sendMessage(chatId, "Нет тренировок", null);
                    }
                    else
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine("Твои тренировки:");

                        //sortedDrills.Select((drill, index) => sb.AppendLine($"{index + 1}. {drill.Date} {drill.DisciplineName}, сложность - {drill.Difficulty}," +
                        //                                            $"тренер - {drill.TrainerDesc}"));

                        foreach (var drill in sortedDrills)
                        {
                            sb.AppendLine($"{ drill.Date} { drill.DisciplineName}, сложность - { drill.Difficulty}, тренер - {drill.TrainerDesc}");
                        }

                        _sendMessage(chatId, sb.ToString(), null);
                    }
                }
            }
        }
    }
}
