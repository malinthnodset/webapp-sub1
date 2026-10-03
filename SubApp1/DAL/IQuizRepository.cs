using SubApp1.Models;

namespace SubApp1.DAL;

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>> GetAll(); // Task == async
    
    Task<IEnumerable<Course>> GetAllCourses(); // for selecting a course the quiz belongs to
    Task<Quiz?> GetById(int id); // ? in case the quiz does not exist
    Task<Quiz?> GetQuizForTakingAsync(int id);
    Task AddAttemptAsync(QuizAttempt attempt);
    Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int studentId);
    Task Create(Quiz quiz);
    Task Delete(int id);

    // Update too?
}