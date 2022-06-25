using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.InMemoryData.EntitiesDB;

namespace OtusTelegramBot.InMemoryData.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IRolesRepository _rolesRepository;

        // Это нормально запихнуть сюда IRolesRepository?
        public UsersRepository(IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository ?? throw new ArgumentNullException(nameof(rolesRepository));
        }

        public User Create(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userToSave = new UserDB
            {
                Id = GetNextId(),
                TelegramId = user.TelegramId,
                Name = user.Name,
                RoleId = user.Role.Id
            };

            Data.Users.Add(userToSave);

            var savedUser = new User()
            {
                Id = userToSave.Id,
                TelegramId = user.TelegramId,
                Name = user.Name,
                Role = user.Role
            };

            return savedUser;
        }

        public User Get(string telegramId)
        {
            var userDB = Data.Users.FirstOrDefault(user => IsTelegramIdsEqual(user.TelegramId, telegramId));

            // А дальше завяжем логику на null, вряд ли хорошо. Если кидать исключение, то завязывать логику на ошибке? ИЛи сделать отдельно метод IsUserExists, но тогда два запроса к БД
            if (userDB is null)
            {
                return null;
            }

            var user = new User()
            {
                Id = userDB.Id,
                TelegramId = userDB.TelegramId,
                Name = userDB.Name,
                Role = _rolesRepository.Get(userDB.RoleId)
            };

            return user;
        }

        public User Get(int id)
        {
            var userDB = Data.Users.FirstOrDefault(user => user.Id == id) ?? throw new Exception();

            var user = new User()
            {
                Id = userDB.Id,
                TelegramId = userDB.TelegramId,
                Name = userDB.Name,
                Role = _rolesRepository.Get(userDB.RoleId)
            };

            return user;
        }

        public List<User> Get(List<int> ids)
        {
            var userDBs = Data.Users.Where(user => ids.Contains(user.Id)) ?? throw new Exception();

            var users = userDBs.Select(user => new User()
            {
                Id = user.Id,
                TelegramId = user.TelegramId,
                Name = user.Name,
                Role = _rolesRepository.Get(user.RoleId)
            }).ToList();

            return users;
        }

        public bool IsExists(string telegramId) => Data.Users.Exists(user => IsTelegramIdsEqual(user.TelegramId, telegramId));

        private int GetNextId() => Data.Users.Select(x => x.Id).Max() + 1;

        private bool IsTelegramIdsEqual(string firstTelegramId, string secondTelegramId) => string.Equals(firstTelegramId, secondTelegramId, StringComparison.InvariantCultureIgnoreCase);
    }
}
