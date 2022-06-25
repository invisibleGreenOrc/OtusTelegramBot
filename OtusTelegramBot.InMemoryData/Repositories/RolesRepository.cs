using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;

namespace OtusTelegramBot.InMemoryData.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        public Role Get(int id)
        {
            var roleDB = Data.Roles.FirstOrDefault(role => role.Id == id) ?? throw new Exception();

            var role = new Role()
            {
                Id = roleDB.Id,
                Name = roleDB.Name
            };

            return role;
        }

        public List<Role> GetAll()
        {
            var roles = new List<Role>();

            roles = Data.Roles.Select(role => new Role
            {
                Id = role.Id,
                Name = role.Name
            }).ToList();

            return roles;
        }
    }
}