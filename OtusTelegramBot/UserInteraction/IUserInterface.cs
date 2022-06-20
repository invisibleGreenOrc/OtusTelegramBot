namespace OtusTelegramBot.Services.UserInteraction
{
    public interface IUserInterface
    {
        string GetInput();
        void PostOutput(string output);
    }
}
