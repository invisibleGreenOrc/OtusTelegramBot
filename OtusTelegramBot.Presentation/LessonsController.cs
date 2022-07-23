using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Presentation.Model;
using OtusTelegramBot.Services.Abstractions;

namespace OtusTelegramBot.Presentation
{
    public class LessonsController
    {
        private readonly ILessonsService _lessonsService;
        private readonly IUsersService _usersService;

        public LessonsController(ILessonsService lessonsService, IUsersService usersService)
        {
            _lessonsService = lessonsService ?? throw new ArgumentNullException(nameof(lessonsService));
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        public List<LessonVM> GetFutureLessons()
        {
            var lessons = _lessonsService.GetFutureLessons();

            var lessonVMs = lessons.Select(lesson => new LessonVM()
            {
                Id = lesson.Id,
                DisciplineName = _lessonsService.GetDiscipline(lesson.Discipline.Id).Name,
                Difficulty = GetDifficultyName(lesson.Difficulty),
                Date = lesson.Date,
                TrainerDesc = lesson.Trainer.Name
            }).ToList();

            return lessonVMs;
        }

        public List<DisciplineVM> GetAllDisciplines()
        {
            var disciplines = _lessonsService.GetAllDisciplines();

            var disciplinesVMs = disciplines.Select(discipline => new DisciplineVM()
            {
                Id = discipline.Id,
                Name = discipline.Name
            }).ToList();

            return disciplinesVMs;
        }

        public List<LevelVM> GetDifficultyLevels()
        {
            var levels = new List<LevelVM>();

            for (int i = 0; i < 3; i++)
            {
                levels.Add(new LevelVM() { Id = i, Name = GetDifficultyName((Difficulty)i) });
            }

            return levels;
        }

        private string GetDifficultyName(Difficulty difficulty)
        {
            var name = string.Empty;

            switch (difficulty)
            {
                case Difficulty.Easy:
                    name = "Легкий";
                    break;
                case Difficulty.Normal:
                    name = "Средний";
                    break;
                case Difficulty.Hard:
                    name = "Сложный";
                    break;
            }

            return name;
        }

        public void AddLesson(LessonForCreatingVM newLesson)
        {
            //var role = _usersService.GetRole(newUser.RoleId);

            var lesson = new Lesson()
            {
                Discipline = _lessonsService.GetDiscipline(newLesson.DisciplineId),
                Difficulty = (Difficulty)newLesson.DifficultyId,
                Date = new DateTime(newLesson.Date.Year, newLesson.Date.Month, newLesson.Date.Day, newLesson.Time.Hour, newLesson.Time.Minute, 0),
                Trainer = _usersService.GetUser(newLesson.TrainerId)
            };

            _lessonsService.CreateLesson(lesson);
        }

        public void AddLessonParticipant(int lessonId, long userId)
        {
            _lessonsService.AddLessonParticipant(lessonId, userId);
        }
    }
}
