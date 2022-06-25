using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Presentation.Model;
using OtusTelegramBot.Services.Abstractions;

namespace OtusTelegramBot.Presentation
{
    public class UsersController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        public UserVM GetUser(string telegramId)
        {
            var user = _usersService.GetUser(telegramId);

            // еще хрень
            if (user is null)
            {
                return null;
            }

            var userVM = new UserVM()
            {
                Id = user.Id,
                TelegramId = user.TelegramId,
                Name = user.Name,
                RoleName = user.Role.Name
            };

            return userVM;
        }

        public void AddUser(UserForCreatingVM newUser)
        {
            var role = _usersService.GetRole(newUser.RoleId);

            var user = new User()
            {
                Name = newUser.Name,
                TelegramId = newUser.TelegramId,
                Role = role
            };

            _usersService.CreateUser(user);
        }
    }
}
