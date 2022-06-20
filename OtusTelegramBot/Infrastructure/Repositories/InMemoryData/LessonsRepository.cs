using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.Infrastructure.EntitiesDB;
using OtusTelegramBot.InMemoryData;

namespace OtusTelegramBot.Infrastructure.Repositories.InMemoryData
{
    internal class LessonsRepository : ILessonsRepository
    {
        private readonly IDisciplinesRepository _disciplinesRepository;
        private readonly IUsersRepository _usersRepository;

        public LessonsRepository(IDisciplinesRepository disciplinesRepository, IUsersRepository usersRepository)
        {
            _disciplinesRepository = disciplinesRepository ?? throw new ArgumentNullException(nameof(disciplinesRepository));
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public Lesson Get(int id)
        {
            var lessonDB = Data.Lessons.FirstOrDefault(lesson => lesson.Id == id) ?? throw new Exception();

            var lesson = new Lesson()
            {
                Id = lessonDB.Id,
                Discipline = _disciplinesRepository.Get(lessonDB.DisciplineId),
                Difficulty = (Difficulty)lessonDB.DifficultyId,
                Date = lessonDB.Date,
                Trainer = _usersRepository.Get(lessonDB.TrainerId),
                Trainee = GetLessonTrainees(lessonDB.Id)
            };

            return lesson;
        }

        public List<Lesson> GetFutureLessons()
        {
            var lessonDBs = Data.Lessons.Where(lesson => lesson.Date > DateTime.Now) ?? throw new Exception();

            var lessons = lessonDBs.Select(lessonDB => new Lesson()
            {
                Id = lessonDB.Id,
                Discipline = _disciplinesRepository.Get(lessonDB.DisciplineId),
                Difficulty = (Difficulty)lessonDB.DifficultyId,
                Date = lessonDB.Date,
                Trainer = _usersRepository.Get(lessonDB.TrainerId),
                Trainee = GetLessonTrainees(lessonDB.Id)
            }).ToList();

            return lessons;
        }

        private List<User> GetLessonTrainees(int lessonId)
        {
            var traineesId = Data.LessonTrainees.Where(trainee => trainee.LessonId == lessonId).Select(trainee => trainee.TraineeId).ToList();
            var lessonTrainees = _usersRepository.Get(traineesId);
            return lessonTrainees;
        }
    }
}
