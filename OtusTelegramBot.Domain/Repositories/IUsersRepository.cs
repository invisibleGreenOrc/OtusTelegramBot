using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Domain.Repositories
{
    public interface IUsersRepository
    {
        User Get(string telegramId);
        User Get(int id);
        public List<User> Get(List<int> ids);
        User Create(User user);
        bool IsExists(string telegramId);
    }
}
