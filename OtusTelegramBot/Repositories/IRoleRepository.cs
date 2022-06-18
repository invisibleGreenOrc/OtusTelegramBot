using OtusTelegramBot.Domain;

namespace OtusTelegramBot.Repositories
{
    public interface IRolesRepository
    {
        List<Role> GetAll();
    }
}
