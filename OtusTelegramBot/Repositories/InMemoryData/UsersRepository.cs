using OtusTelegramBot.Domain;
using OtusTelegramBot.InMemoryData;

namespace OtusTelegramBot.Repositories.InMemoryData
{
    internal class UsersRepository : IUsersRepository
    {
        public User Create(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userToSave = new User
            {
                Id = GetNextId(),
                TelegramId = user.TelegramId,
                Name = user.Name,
                Role = user.Role
            };

            Data.Users.Add(userToSave);
            
            return userToSave;
        } 

        public User Get(string telegramId) => Data.Users.Find(user => IsTelegramIdsEqual(user.TelegramId, telegramId));

        public bool IsExists(string telegramId) => Data.Users.Exists(user => user.TelegramId == telegramId);

        private int GetNextId() => Data.Users.Select(x => x.Id).Max() + 1;

        private bool IsTelegramIdsEqual(string firstTelegramId, string secondTelegramId) => string.Equals(firstTelegramId, secondTelegramId, StringComparison.InvariantCultureIgnoreCase);
    }
}
