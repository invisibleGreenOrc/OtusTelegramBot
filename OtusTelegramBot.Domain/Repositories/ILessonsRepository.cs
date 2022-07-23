using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Domain.Repositories
{
    public interface ILessonsRepository
    {
        Lesson Get(int id);
        public List<Lesson> GetFutureLessons();
        public List<Lesson> GetFutureLessonsByTrainer(long trainerId);
        public List<Lesson> GetFutureLessonsByTrainee(long traineeId);
        public void Create(Lesson lesson);
        public void AddLessonParticipant(int lessonId, long userId);
    }
}
