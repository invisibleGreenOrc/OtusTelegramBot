namespace OtusTelegramBot.Presentation
{
    public class ConsoleIO : IUserInterface
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
