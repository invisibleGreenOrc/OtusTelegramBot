using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;

namespace OtusTelegramBot.InMemoryData.Repositories
{
    public class DisciplinesRepository : IDisciplinesRepository
    {
        public Discipline Get(int id)
        {
            var disciplineDB = Data.Disciplines.FirstOrDefault(discipline => discipline.Id == id) ?? throw new Exception();

            var discipline = new Discipline()
            {
                Id = disciplineDB.Id,
                Name = disciplineDB.Name
            };

            return discipline;
        }

        public List<Discipline> GetAll()
        {
            var disciplines = new List<Discipline>();

            disciplines = Data.Disciplines.Select(role => new Discipline
            {
                Id = role.Id,
                Name = role.Name
            }).ToList();

            return disciplines;
        }
    }
}
