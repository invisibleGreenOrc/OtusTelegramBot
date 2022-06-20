using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Domain.Repositories
{
    public interface IRolesRepository
    {
        List<Role> GetAll();
        Role Get(int id);
    }
}
