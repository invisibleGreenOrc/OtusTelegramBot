using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.Infrastructure.Repositories.InMemoryData;
using OtusTelegramBot.Services;
using OtusTelegramBot.Services.UserInteraction;
using OtusTelegramBot.UserInteraction;
using OtusTelegramBot.UserInteraction.Model;
using System.Text;

namespace OtusTelegramBot
{
    public class Program
    {
        private static IUserInterface _userInterface = new ConsoleIO();

        private static IRolesRepository _roleRepository = new RolesRepository();
        private static IUsersRepository _usersRepository = new UsersRepository(_roleRepository);
        private static IDisciplinesRepository _disciplinesRepository = new DisciplinesRepository();
        private static ILessonsRepository _lessonsRepository = new LessonsRepository(_disciplinesRepository, _usersRepository);

        private static IUsersService _userService = new UsersService(_usersRepository, _roleRepository);
        private static ILessonsService _lessonsService = new LessonsService(_lessonsRepository, _disciplinesRepository);
        

        private static UsersController _usersController = new UsersController(_userService);
        private static LessonsController _lessonsController = new LessonsController(_lessonsService, _userService);

        static void Main(string[] args)
        {
            _userInterface.PostOutput("Введите свой telegramId");

            var telegramId = _userInterface.GetInput();

            GetOrCreateUser(telegramId);

            while (true)
            {
                Console.WriteLine("Введите команду");

                var userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "Смотреть тренировки":
                        Console.WriteLine(GetFutureLessons());
                        break;
                    default:
                        break;
                }
            }
}

        // Что-то не то
        public static UserVM GetOrCreateUser(string telegramId)
        {
            var user = _usersController.GetUser(telegramId);

            if (user is null)
            {
                CreateNewUser(telegramId);
            }

            user = _usersController.GetUser(telegramId);
            GreetUser(user);

            return user;
        }

        private static void CreateNewUser(string telegramId)
        {
            var userToCreate = GetUserData();
            userToCreate.TelegramId = telegramId;

            _usersController.AddUser(userToCreate);

            _userInterface.PostOutput($"Новый пользователь для {userToCreate.TelegramId} создан!");
        }

        private static UserForCreating GetUserData()
        {
            var user = new UserForCreating();

            _userInterface.PostOutput("Введите ваше имя");
            user.Name = _userInterface.GetInput();

            var roles = _userService.GetAllRoles();
            var rolesList = new StringBuilder();

            // Привязываться к Id вряд ли хорошая идея
            foreach (var role in roles)
            {
                rolesList.Append($"{role.Id}. {role.Name}\n");
            }

            var roleId = 0;

            do
            {
                _userInterface.PostOutput($"Выберите роль. Выберите номер роли из списка:\n{rolesList}");
            }
            while (!int.TryParse(_userInterface.GetInput(), out roleId) || !roles.Exists(role => role.Id == roleId));

            user.RoleId = roleId;

            return user;
        }

        private static void GreetUser(UserVM user)
        {
            var output = $"{user.Name}, привет! Что будем делать дальше?";
            _userInterface.PostOutput(output);
        }

        private static string GetFutureLessons()
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