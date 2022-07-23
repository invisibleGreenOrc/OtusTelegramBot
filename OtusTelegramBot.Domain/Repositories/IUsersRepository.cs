using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Domain.Repositories
{
    public interface IUsersRepository
    {
        User Get(long userId);
        User Get(int id);
        public List<User> Get(List<long> ids);
        User Create(User user);
        bool IsExists(long userId);
    }
}
