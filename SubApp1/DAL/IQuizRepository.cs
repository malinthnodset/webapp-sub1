using SubApp1.Models;

namespace SubApp1.DAL;

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>?> GetAllQuizzes(); // Task == async. Nullable in cas of db-connection error
    Task<Quiz?> GetQuizById(int id); // ? in case the quiz does not exist
    Task<bool> Create(Quiz quiz); // setting datatype to bool = we get confirmation of success (true/false), so avoids silent failing
    Task<bool> Update (Quiz quiz); // not implemented in view yet
    Task<bool> Delete(int id);

    // Related to taking a quiz:
    Task<Quiz?> GetQuizForTakingAsync(int id);
    Task AddAttemptAsync(QuizAttempt attempt);
    Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int studentId);
}