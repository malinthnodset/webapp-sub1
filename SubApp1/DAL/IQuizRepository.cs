using SubApp1.Models;

namespace SubApp1.DAL;

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>> GetAllQuizzes();
    Task<Quiz?> GetQuizById(int id);
    Task<bool> Create(Quiz quiz);
    Task<bool> Update(Quiz quiz);
    Task<bool> Delete(int id);
    
    
    // Related to taking a quiz:
    Task<Quiz?> GetQuizForTakingAsync(int id);
    Task AddAttemptAsync(QuizAttempt attempt); // add a new attempt to the database
    Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int userId); // get all attempts for a given user and quiz (check if they have already taken it)
    Task<QuizAttempt?> GetLatestAttemptWithAnswersAsync(int quizId, int userId); // dashboard: get the latest attempt for a given user and quiz, including the answers they gave (to show their best score and completion status)

}