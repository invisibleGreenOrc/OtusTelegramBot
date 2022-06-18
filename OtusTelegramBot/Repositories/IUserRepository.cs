using OtusTelegramBot.Domain;

namespace OtusTelegramBot.Repositories
{
    public interface IUsersRepository
    {
        User Get(string telegramId);
        //User GetByRole(int roleId);
        User Create(User user);
        bool IsExists(string telegramId);
    }
}
