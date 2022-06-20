namespace OtusTelegramBot.UserInteraction.Model
{
    public class LessonVM
    {
        public int Id { get; set; }
        public string DisciplineName { get; set; }
        public string Difficulty { get; set; }
        public DateTime Date { get; set; }
        public string TrainerDesc { get; set; }
    }
}
