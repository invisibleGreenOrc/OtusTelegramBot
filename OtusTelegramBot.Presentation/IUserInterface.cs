namespace OtusTelegramBot.Presentation
{
    public interface IUserInterface
    {
        string GetInput();
        void PostOutput(string output);
    }
}
