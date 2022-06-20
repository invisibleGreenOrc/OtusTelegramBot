using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.Infrastructure.EntitiesDB;
using OtusTelegramBot.InMemoryData;

namespace OtusTelegramBot.Infrastructure.Repositories.InMemoryData
{
    public class UsersRepository : IUsersRepository
    {
        public IRolesRepository _rolesRepository { get; set; }

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
            var userDB = Data.Users.Find(user => IsTelegramIdsEqual(user.TelegramId, telegramId));

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
            var userDB = Data.Roles.FirstOrDefault(user => user.Id == id) ?? throw new Exception();

            var user = new User()
            {
                Id = userDB.Id,
                Name = userDB.Name
            };

            return user;
        }

        public bool IsExists(string telegramId) => Data.Users.Exists(user => IsTelegramIdsEqual(user.TelegramId, telegramId));

        private int GetNextId() => Data.Users.Select(x => x.Id).Max() + 1;

        private bool IsTelegramIdsEqual(string firstTelegramId, string secondTelegramId) => string.Equals(firstTelegramId, secondTelegramId, StringComparison.InvariantCultureIgnoreCase);
    }
}
