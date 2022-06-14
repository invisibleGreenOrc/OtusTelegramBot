using OtusTelegramBot.Model;

namespace OtusTelegramBot
{
    interface IDataAccess
    {
        IEnumerable<Role> GetRoles();
        IEnumerable<Discipline> GetDisciplines();
        void AddUserProfile(UserProfile profile);
        void AddLesson(Lesson lesson);
        void AddLessonParticipant(Lesson lesson, UserProfile trainee);
        IEnumerable<Lesson> GetLessons();
        IEnumerable<Lesson> GetLessonsByTrainer(UserProfile trainer);
        IEnumerable<Lesson> GetLessonsByTrainee(UserProfile trainee);
    }
}
 