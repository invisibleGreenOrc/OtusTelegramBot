﻿using OtusTelegramBot.Domain.Entities;

namespace OtusTelegramBot.Domain
{
    internal class Lesson
    {
        public int Id { get; set; }
        public Discipline Discipline { get; set; }
        public Difficulty Difficulty { get; set; }
        public DateTime Date { get; set; }
        public User Trainer { get; set; }
        public List<User> Trainee { get; set; }
    }
}
