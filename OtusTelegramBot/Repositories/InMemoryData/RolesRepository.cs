using OtusTelegramBot.Domain;
using OtusTelegramBot.InMemoryData;

namespace OtusTelegramBot.Repositories.InMemoryData
{
    internal class RolesRepository : IRolesRepository
    {
        public List<Role> GetAll() => Data.Roles;
    }
}
