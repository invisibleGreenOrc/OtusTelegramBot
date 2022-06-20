using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Services;
using OtusTelegramBot.UserInteraction.Model;

namespace OtusTelegramBot.UserInteraction
{
    public class LessonsController
    {
        private readonly ILessonsService _lessonsService;

        public LessonsController(ILessonsService lessonsService, IUsersService usersService)
        {
            _lessonsService = lessonsService ?? throw new ArgumentNullException(nameof(lessonsService));
        }

        public List<LessonVM> GetFutureLessons()
        {
            var lessons = _lessonsService.GetFutureLessons();

            var lessonVMs = lessons.Select(lesson => new LessonVM()
            {
                Id = lesson.Id,
                DisciplineName = _lessonsService.GetDiscipline(lesson.Id).Name,
                Difficulty = GetDifficultyName(lesson.Difficulty),
                Date = lesson.Date,
                TrainerDesc = lesson.Trainer.Name
            }).ToList();

            return lessonVMs;
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
    }
}
