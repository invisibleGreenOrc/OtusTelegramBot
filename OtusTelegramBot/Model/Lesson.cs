namespace OtusTelegramBot.Model
{
    internal class Lesson
    {
        public int Id { get; set; }
        public Discipline Discipline { get; set; }
        public Difficulty Difficulty { get; set; }
        public DateTime Date { get; set; }
        public UserProfile Trainer { get; set; }
        public List<UserProfile> Trainee { get; set; }
    }
}
