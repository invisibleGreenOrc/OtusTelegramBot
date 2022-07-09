using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Services.Abstractions
{
    public interface ILessonsService
    {
        public List<Lesson> GetFutureLessons();
        public Discipline GetDiscipline(int id);
        public List<Discipline> GetAllDisciplines();

        public void CreateLesson(Lesson newLesson);
    }
}
