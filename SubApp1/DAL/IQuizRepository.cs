using SubApp1.Models;

namespace SubApp1.DAL;

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>?> GetAllQuizzes();
    Task<Quiz?> GetQuizById(int id);
    Task<bool> Create(Quiz quiz);
    Task<bool> Update(Quiz quiz);
    Task<bool> Delete(int id);
    
    // Related to taking a quiz:
    Task<Quiz?> GetQuizForTakingAsync(int id);
    Task AddAttemptAsync(QuizAttempt attempt); // add a new attempt to the database
    Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int studentId); // get all attempts for a given student and quiz (check if they have already taken it)
}