namespace OtusTelegramBot.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
    }
}
