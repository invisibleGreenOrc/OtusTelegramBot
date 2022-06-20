using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Services
{
    public interface IUsersService
    {
        public User GetUser(string telegramId);
        public void CreateUser(User newUser);
        public List<Role> GetAllRoles();
        public Role GetRole(int roleId);
    }
}
