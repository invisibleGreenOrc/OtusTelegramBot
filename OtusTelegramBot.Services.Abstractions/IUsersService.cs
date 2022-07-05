using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Services.Abstractions
{
    public interface IUsersService
    {
        public User GetUser(long userId);
        public void CreateUser(User newUser);
        public List<Role> GetAllRoles();
        public Role GetRole(int roleId);
        public bool IsUserExists(long userId);
    }
}
