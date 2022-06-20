using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.Infrastructure.Repositories.InMemoryData;
using OtusTelegramBot.Services;
using OtusTelegramBot.Services.UserInteraction;
using System.Text;

namespace OtusTelegramBot
{
    public class Program
    {
        private static IUserInterface _userInterface = new ConsoleIO();
        private static IRolesRepository _roleRepository = new RolesRepository();
        private static IUsersService _userService = new UsersService(new UsersRepository(_roleRepository), _roleRepository);

        static void Main(string[] args)
        {
            

            while (true)
            {
                _userInterface.PostOutput("Введите свой telegramId");

                var telegramId = _userInterface.GetInput();

                GetOrCreateUser(telegramId);
            }
}

        // Что-то не то
        public static User GetOrCreateUser(string telegramId)
        {
            var user = _userService.GetUser(telegramId) ?? CreateNewUser(telegramId);

            GreetUser(user);

            return user;
        }

        private static User CreateNewUser(string telegramId)
        {
            var userToCreate = GetUserData();
            userToCreate.TelegramId = telegramId;

            _userService.CreateUser(userToCreate);

            _userInterface.PostOutput($"Новый пользователь для {userToCreate.TelegramId} создан!");

            return userToCreate;
        }

        private static User GetUserData()
        {
            var user = new User();

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

            user.Role = roles.Find(role => role.Id == roleId);

            return user;
        }

        private static void GreetUser(User user)
        {
            var output = $"{user.Name}, привет! Что будем делать дальше?";
            _userInterface.PostOutput(output);
        }
    }
}