namespace OtusTelegramBot.Presentation.Model
{
    public class LessonForCreatingVM
    {
        public int DisciplineId { get; set; }
        public int DifficultyId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public int TrainerId { get; set; }
    }
}
