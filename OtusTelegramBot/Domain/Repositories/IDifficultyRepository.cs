using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Domain.Repositories
{
    public interface IDifficultyRepository
    {
        List<Difficulty> GetAll();
        Difficulty Get(int id);
    }
}
