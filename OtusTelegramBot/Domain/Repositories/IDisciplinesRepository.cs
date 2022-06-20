using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Domain.Repositories
{
    public interface IDisciplinesRepository
    {
        List<Discipline> GetAll();
        Discipline Get(int id);
    }
}
