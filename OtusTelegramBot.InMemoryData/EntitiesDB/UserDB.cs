namespace OtusTelegramBot.InMemoryData.EntitiesDB
{
    public class UserDB
    {
        public int Id { get; set; }
        public string TelegramId { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
    }
}
