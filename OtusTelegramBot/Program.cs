using OtusTelegramBot.Infrastructure.Repositories.InMemoryData;
using OtusTelegramBot.Services;
using OtusTelegramBot.Services.UserInteraction;

namespace OtusTelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IUserInterface userInterface = new ConsoleIO();
            var service = new UserService(new UsersRepository(), new RolesRepository(), userInterface);

            while (true)
            {
                userInterface.PostOutput("Введите свой telegramId");

                var telegramId = userInterface.GetInput();

                service.GetOrCreateUser(telegramId);
            }
        }
    }
}