using OtusTelegramBot.Infrastructure.EntitiesDB;

namespace OtusTelegramBot.InMemoryData
{
    public static class Data
    {
        public static List<RoleDB> Roles { get; set; } = new()
        {
            new RoleDB { Id = 0, Name =  "Trainer" },
            new RoleDB { Id = 1, Name = "Trainee" }
        };

        public static List<UserDB> Users { get; set; } = new()
        {
            new UserDB { Id = 0, TelegramId = "trainer1", Name = "Тренер1", RoleId = 0 },
            new UserDB { Id = 1, TelegramId = "trainer2", Name = "Тренер2", RoleId = 0 },
            new UserDB { Id = 2, TelegramId = "trainee1", Name = "Ученик1", RoleId = 1 },
            new UserDB { Id = 3, TelegramId = "trainee2", Name = "Ученик2", RoleId = 1 }
        };

        public static List<DifficultyDB> Difficulties { get; set; } = new()
        {
            new DifficultyDB { Id = 0, Name = "Легкий" },
            new DifficultyDB { Id = 1, Name = "Средний" },
            new DifficultyDB { Id = 2, Name = "Сложный" }
        };

        public static List<DisciplinesDB> Disciplines { get; set; } = new()
        {
            new DisciplinesDB { Id = 0, Name = "Football" },
            new DisciplinesDB { Id = 1, Name = "Volleyball" },
            new DisciplinesDB { Id = 2, Name = "Athletics" },
            new DisciplinesDB { Id = 3, Name = "Weightlifting" },
            new DisciplinesDB { Id = 4, Name = "Yoga" }
        };

        public static List<LessonDB> Lessons { get; set; } = new()
        {
            new LessonDB { Id = 0, DisciplineId = 0, DifficultyId = 0, Date = new DateTime(2021, 1, 1, 10, 30, 0), TrainerId = 0},
            new LessonDB { Id = 1, DisciplineId = 0, DifficultyId = 2, Date = new DateTime(2021, 1, 2, 10, 30, 0), TrainerId = 0},
            new LessonDB { Id = 2, DisciplineId = 1, DifficultyId = 0, Date = new DateTime(2021, 1, 3, 10, 30, 0), TrainerId = 0},
            new LessonDB { Id = 3, DisciplineId = 2, DifficultyId = 1, Date = new DateTime(2021, 1, 3, 12, 30, 0), TrainerId = 0},
            new LessonDB { Id = 4, DisciplineId = 3, DifficultyId = 2, Date = new DateTime(2021, 1, 1, 10, 30, 0), TrainerId = 1},
            new LessonDB { Id = 5, DisciplineId = 4, DifficultyId = 0, Date = new DateTime(2021, 1, 3, 12, 30, 0), TrainerId = 1}
        };
    }
}
