using OtusTelegramBot.Domain;

namespace OtusTelegramBot.InMemoryData
{
    public static class Data
    {
        public static List<Role> Roles = new()
        {
            new Role { Id = 0, Name =  "Trainer" },
            new Role { Id = 1, Name = "Trainee" }
        };

        public static List<User> Users = new()
        {
            new User { Id = 0, TelegramId = "termit", Name = "Иван", Role = Roles[0] },
            new User { Id = 1, TelegramId = "bug", Name = "Степан", Role = Roles[1] }
        };
    }
}
