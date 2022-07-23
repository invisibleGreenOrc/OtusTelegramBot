using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Services.Abstractions
{
    public interface ILessonsService
    {
        public List<Lesson> GetFutureLessons();
        public List<Lesson> GetFutureLessonsForUser(long userId);
        public Discipline GetDiscipline(int id);
        public List<Discipline> GetAllDisciplines();
        public void CreateLesson(Lesson newLesson);
        public void AddLessonParticipant(int lessonId, long userId);
    }
}
