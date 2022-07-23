using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.InMemoryData.EntitiesDB;

namespace OtusTelegramBot.InMemoryData.Repositories
{
    public class LessonsRepository : ILessonsRepository
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
                Trainees = GetLessonTrainees(lessonDB.Id)
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
                Trainees = GetLessonTrainees(lessonDB.Id)
            }).ToList();

            return lessons;
        }

        public List<Lesson> GetFutureLessonsByTrainer(long trainerId)
        {
            var lessonDBs = Data.Lessons.Where(lesson => lesson.Date > DateTime.Now && lesson.TrainerId == trainerId);

            var lessons = lessonDBs.Select(lessonDB => new Lesson()
            {
                Id = lessonDB.Id,
                Discipline = _disciplinesRepository.Get(lessonDB.DisciplineId),
                Difficulty = (Difficulty)lessonDB.DifficultyId,
                Date = lessonDB.Date,
                Trainer = _usersRepository.Get(lessonDB.TrainerId),
                Trainees = GetLessonTrainees(lessonDB.Id)
            }).ToList();

            return lessons;
        }

        public List<Lesson> GetFutureLessonsByTrainee(long traineeId)
        {
            var lessonIds = Data.LessonTrainees.Where(x => x.TraineeId == traineeId).Select(x => x.LessonId);
            var lessonDBs = Data.Lessons.IntersectBy(lessonIds, x => x.Id);

            var lessons = lessonDBs.Select(lessonDB => new Lesson()
            {
                Id = lessonDB.Id,
                Discipline = _disciplinesRepository.Get(lessonDB.DisciplineId),
                Difficulty = (Difficulty)lessonDB.DifficultyId,
                Date = lessonDB.Date,
                Trainer = _usersRepository.Get(lessonDB.TrainerId),
                Trainees = GetLessonTrainees(lessonDB.Id)
            }).ToList();

            return lessons;
        }

        public void Create(Lesson lesson)
        {
            if (lesson is null)
            {
                throw new ArgumentNullException(nameof(lesson));
            }

            var lessonToSave = new LessonDB
            {
                Id = GetNextId(),
                DisciplineId = lesson.Discipline.Id,
                DifficultyId = (int)lesson.Difficulty,
                Date = lesson.Date,
                TrainerId = lesson.Trainer.Id
            };

            Data.Lessons.Add(lessonToSave);
        }

        public void AddLessonParticipant(int lessonId, long userId)
        {
            var lessonParticipant = new LessonParticipantsDB
            {
                Id = Data.LessonTrainees.Select(x => x.Id).Max() + 1,
                LessonId = lessonId,
                TraineeId = userId
            };

            Data.LessonTrainees.Add(lessonParticipant);
        }

        private List<User> GetLessonTrainees(int lessonId)
        {
            var traineesId = Data.LessonTrainees.Where(trainee => trainee.LessonId == lessonId).Select(trainee => trainee.TraineeId).ToList();
            var lessonTrainees = _usersRepository.Get(traineesId);
            return lessonTrainees;
        }

        private int GetNextId() => Data.Lessons.Select(x => x.Id).Max() + 1;
    }
}
