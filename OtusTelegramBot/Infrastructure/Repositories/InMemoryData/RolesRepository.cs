using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.InMemoryData;

namespace OtusTelegramBot.Infrastructure.Repositories.InMemoryData
{
    internal class RolesRepository : IRolesRepository
    {
        public List<Role> GetAll() => Data.Roles;
    }
}
