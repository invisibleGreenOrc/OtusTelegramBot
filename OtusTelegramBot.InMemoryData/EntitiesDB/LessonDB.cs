namespace OtusTelegramBot.InMemoryData.EntitiesDB
{
    public class LessonDB
    {
        public int Id { get; set; }
        public int DisciplineId { get; set; }
        public int DifficultyId { get; set; }
        public DateTime Date { get; set; }
        public int TrainerId { get; set; }
    }
}
