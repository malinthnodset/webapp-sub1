using SubApp1.Models; // [LEARN] Entities

namespace SubApp1.DAL; // [LEARN] Data access namespace

public interface IQuizRepository // [LEARN] Contract for quiz data access
{
    Task<IEnumerable<Quiz>?> GetAllQuizzes(); // [LEARN] Get all quizzes
    Task<Quiz?> GetQuizById(int id); // [LEARN] Get quiz by id
    Task<bool> Create(Quiz quiz); // [LEARN] Create quiz
    Task<bool> Update(Quiz quiz); // [LEARN] Update quiz
    Task<bool> Delete(int id); // [LEARN] Delete quiz
    
    // [LEARN] Related to taking a quiz:
    Task<Quiz?> GetQuizForTakingAsync(int id); // [LEARN] Load quiz for taking
    Task AddAttemptAsync(QuizAttempt attempt); // [LEARN] add a new attempt to the database
    Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int studentId); // [LEARN] get all attempts for a given student and quiz (check if they have already taken it)
}
