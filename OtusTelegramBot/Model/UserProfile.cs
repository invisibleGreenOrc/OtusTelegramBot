namespace OtusTelegramBot.Model
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string TelegramId { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
    }
}
