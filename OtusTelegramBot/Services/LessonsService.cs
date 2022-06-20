﻿using OtusTelegramBot.Domain.Entities;
using OtusTelegramBot.Domain.Repositories;

namespace OtusTelegramBot.Services
{
    public class LessonsService : ILessonsService
    {
        private readonly ILessonsRepository _lessonsRepository;
        private readonly IDisciplinesRepository _disciplinesRepository;

        public LessonsService(ILessonsRepository lessonsRepository, IDisciplinesRepository disciplinesRepository)
        {
            _lessonsRepository = lessonsRepository ?? throw new ArgumentNullException(nameof(lessonsRepository));
            _disciplinesRepository = disciplinesRepository ?? throw new ArgumentNullException(nameof(disciplinesRepository));
        }

        public List<Lesson> GetFutureLessons()
        {
            return _lessonsRepository.GetFutureLessons();
        }

        public Discipline GetDiscipline(int id)
        {
            return _disciplinesRepository.Get(id);
        }
    }
}
