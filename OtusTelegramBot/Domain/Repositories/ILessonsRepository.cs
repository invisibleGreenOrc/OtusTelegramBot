using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Domain.Repositories
{
    public interface ILessonsRepository
    {
        Lesson Get(int id);
        public List<Lesson> GetFutureLessons();
    }
}
