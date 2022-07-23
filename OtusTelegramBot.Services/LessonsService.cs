using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;
using OtusTelegramBot.Services.Abstractions;

namespace OtusTelegramBot.Services
{
    public class LessonsService : ILessonsService
    {
        private readonly ILessonsRepository _lessonsRepository;
        private readonly IDisciplinesRepository _disciplinesRepository;
        private readonly IUsersService _usersService;

        public LessonsService(ILessonsRepository lessonsRepository, IDisciplinesRepository disciplinesRepository, IUsersService usersService)
        {
            _lessonsRepository = lessonsRepository ?? throw new ArgumentNullException(nameof(lessonsRepository));
            _disciplinesRepository = disciplinesRepository ?? throw new ArgumentNullException(nameof(disciplinesRepository));
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        public List<Lesson> GetFutureLessons()
        {
            return _lessonsRepository.GetFutureLessons();
        }

        public List<Lesson> GetFutureLessonsForUser(long userId)
        {
            var lessons = new List<Lesson>();
            var user = _usersService.GetUser(userId);

            if (user?.Role.Id == 0)
            {
                lessons = _lessonsRepository.GetFutureLessonsByTrainer(user.Id);
            }
            else if (user?.Role.Id == 1)
            {
                lessons = _lessonsRepository.GetFutureLessonsByTrainee(user.Id);
            }

            return lessons;
        }

        public Discipline GetDiscipline(int id)
        {
            return _disciplinesRepository.Get(id);
        }

        public List<Discipline> GetAllDisciplines()
        {
            return _disciplinesRepository.GetAll();
        }

        public void CreateLesson(Lesson newLesson)
        {
            _lessonsRepository.Create(newLesson);
        }

        public void AddLessonParticipant(int lessonId, long userId)
        {
            var user = _usersService.GetUser(userId);
            _lessonsRepository.AddLessonParticipant(lessonId, user.Id);
        }
    }
}
