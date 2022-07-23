using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Domain.Repositories
{
    public interface ILessonsRepository
    {
        Lesson Get(int id);
        public List<Lesson> GetFutureLessons();
        public void Create(Lesson lesson);
        public void AddLessonParticipant(int lessonId, long userId);
    }
}
