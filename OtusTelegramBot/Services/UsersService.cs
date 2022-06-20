using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;

namespace OtusTelegramBot.Services
{
    internal class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IRolesRepository _rolesRepository;

        public UsersService(IUsersRepository usersRepository, IRolesRepository rolesRepository)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            _rolesRepository = rolesRepository ?? throw new ArgumentNullException(nameof(rolesRepository));
        }
     
        public void CreateUser(User user)
        {
            _usersRepository.Create(user);
        }

        public List<Role> GetAllRoles()
        {
            return _rolesRepository.GetAll();
        }

        public User GetUser(string telegramId)
        {
            return _usersRepository.Get(telegramId);
        }

        public Role GetRole(int roleId)
        {
            return _rolesRepository.Get(roleId);
        }
    }
}
