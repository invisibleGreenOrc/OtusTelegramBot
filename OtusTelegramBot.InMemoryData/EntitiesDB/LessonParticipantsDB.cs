namespace OtusTelegramBot.InMemoryData.EntitiesDB
{
    public class LessonParticipantsDB
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public long TraineeId { get; set; }
    }
}
