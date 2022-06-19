using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.Services.UserInteraction;
using System.Text;

namespace OtusTelegramBot.Services
{
    internal class UserService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUserInterface _userInterface;
        private readonly IRolesRepository _rolesRepository;

        public UserService(IUsersRepository usersRepository, IRolesRepository rolesRepository, IUserInterface userInterface)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            _rolesRepository = rolesRepository ?? throw new ArgumentNullException(nameof(rolesRepository));
            _userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
        }
        
        // Что-то не то
        public User GetOrCreateUser(string telegramId)
        {
            var user = _usersRepository.Get(telegramId) ?? CreateNewUser(telegramId);

            GreetUser(user);

            return user;
        }

        private User CreateNewUser(string telegramId)
        {
            var userToCreate = GetUserData();
            userToCreate.TelegramId = telegramId;

            var user = CreateUser(userToCreate);

            _userInterface.PostOutput($"Новый пользователь для { user.TelegramId } создан!");

            return user;
        }

        private User GetUserData()
        {
            var user = new User();

            _userInterface.PostOutput("Введите ваше имя");
            user.Name = _userInterface.GetInput();

            var roles = GetRoles();
            var rolesList = new StringBuilder();
            
            // Привязываться к Id вряд ли хорошая идея
            foreach (var role in roles)
            {
                rolesList.Append($"{ role.Id }. {role.Name}\n");
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

        private User CreateUser(User user)
        {
            var createdUser = _usersRepository.Create(user);
            return createdUser;
        }

        // Такие сообщения где должны быть? В отдельных методах или вообще взаимодействие с пользовательм где-то в отдельном проекте?
        private void GreetUser(User user)
        {
            var output = $"{ user.Name }, привет! Что будем делать дальше?";
            _userInterface.PostOutput(output);
        }

        private List<Role> GetRoles()
        {
            return _rolesRepository.GetAll();
        }
    }
}
