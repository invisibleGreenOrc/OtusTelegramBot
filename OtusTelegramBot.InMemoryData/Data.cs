using OtusTelegramBot.InMemoryData.EntitiesDB;

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
            new UserDB { Id = 0, UserId = 1, Name = "Тренер1", RoleId = 0 },
            new UserDB { Id = 1, UserId = 2, Name = "Тренер2", RoleId = 0 },
            new UserDB { Id = 2, UserId = 3, Name = "Ученик1", RoleId = 1 },
            new UserDB { Id = 3, UserId = 4, Name = "Ученик2", RoleId = 1 }
        };

        public static List<DisciplinesDB> Disciplines { get; set; } = new()
        {
            new DisciplinesDB { Id = 0, Name = "Футбол" },
            new DisciplinesDB { Id = 1, Name = "Волейбол" },
            new DisciplinesDB { Id = 2, Name = "Легкая атлетика" },
            new DisciplinesDB { Id = 3, Name = "Тяжелая атлетика" },
            new DisciplinesDB { Id = 4, Name = "Йога" }
        };

        public static List<LessonDB> Lessons { get; set; } = new()
        {
            new LessonDB { Id = 0, DisciplineId = 0, DifficultyId = 0, Date = new DateTime(2023, 1, 1, 10, 30, 0), TrainerId = 0},
            new LessonDB { Id = 1, DisciplineId = 0, DifficultyId = 2, Date = new DateTime(2023, 1, 2, 10, 30, 0), TrainerId = 0},
            new LessonDB { Id = 2, DisciplineId = 1, DifficultyId = 0, Date = new DateTime(2021, 1, 3, 10, 30, 0), TrainerId = 0},
            new LessonDB { Id = 3, DisciplineId = 2, DifficultyId = 1, Date = new DateTime(2021, 1, 3, 12, 30, 0), TrainerId = 0},
            new LessonDB { Id = 4, DisciplineId = 3, DifficultyId = 2, Date = new DateTime(2021, 1, 1, 10, 30, 0), TrainerId = 1},
            new LessonDB { Id = 5, DisciplineId = 4, DifficultyId = 0, Date = new DateTime(2021, 1, 3, 12, 30, 0), TrainerId = 1}
        };

        public static List<LessonTraineesDB> LessonTrainees { get; set; } = new()
        {
            new LessonTraineesDB {Id = 0, LessonId = 0, TraineeId = 2},
            new LessonTraineesDB {Id = 1, LessonId = 0, TraineeId = 3},
            new LessonTraineesDB {Id = 2, LessonId = 1, TraineeId = 2},
            new LessonTraineesDB {Id = 3, LessonId = 2, TraineeId = 3},
            new LessonTraineesDB {Id = 4, LessonId = 3, TraineeId = 3},
            new LessonTraineesDB {Id = 5, LessonId = 4, TraineeId = 2},
            new LessonTraineesDB {Id = 6, LessonId = 5, TraineeId = 3},
            new LessonTraineesDB {Id = 6, LessonId = 5, TraineeId = 2}
        };
    }
}
