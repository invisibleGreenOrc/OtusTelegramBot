namespace OtusTelegramBot.Services.UserInteraction
{
    internal class ConsoleIO : IUserInterface
    {
        public string GetInput()
        {
            return Console.ReadLine();
        }

        public void PostOutput(string output)
        {
            Console.WriteLine(output);
        }
    }
}
