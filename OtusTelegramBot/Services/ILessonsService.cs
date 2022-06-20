using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Services
{
    public interface ILessonsService
    {
        public List<Lesson> GetFutureLessons();
        public Discipline GetDiscipline(int id);
    }
}
