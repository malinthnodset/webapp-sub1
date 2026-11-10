using Microsoft.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

public class QuizRepository : IQuizRepository
{
    private readonly QuizDbContext _db;
    private readonly ILogger<QuizRepository> _logger;

    public QuizRepository(QuizDbContext db, ILogger<QuizRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Quiz>> GetAllQuizzes()
    {
        try
        {
            return await _db.Quizzes.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] quizzes ToListAsync() failed when GetAll(), error message: {e}", e.Message);
            throw;
        }
    }

    public async Task<Quiz?> GetQuizById(int id)
    {
        try
        {
            return await _db.Quizzes.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] quiz FindAsync(id) failed when GetQuizById for QuizId {QuizId:0000}, error message:", e.Message);
            throw;
        }
    }

    // Creates and saves a quiz. Database errors propagate to the caller.
    public async Task<bool> Create(Quiz quiz)
    {
        try
        {
            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] quiz creation failed for quiz {@quiz}, error message:", e.Message);
            throw;
        }
    }

    // Updates and saves a quiz. Database errors propagate to the caller.
    public async Task<bool> Update(Quiz quiz)
    {
        try
        {
            var existingQuiz = await _db.Quizzes.FindAsync(quiz);
            if (existingQuiz is null)
            {
                return false;
            }

            _db.Quizzes.Update(existingQuiz);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] failed to update quiz of QuizId {@Id}, error message {e}:", quiz.Id, e.Message);
            throw;
        }
    }

    // searches for quiz by it's ID, if it doesn't exist it returns false
    // if it exists, it is deleted and the changes saved
    public async Task<bool> Delete(int id)
    {
        try
        {
            var existingQuiz = await _db.Quizzes.FindAsync(id);
            if (existingQuiz is null)
            {
                return false;
            }

            _db.Quizzes.Remove(existingQuiz);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete quiz {QuizId}", id);
            throw;
        }
    }

    // TAKE QUIZ METHODS
    public async Task<Quiz?> GetQuizForTakingAsync(int Id)
    {
        try
        {
            return await _db.Quizzes
                .AsNoTracking()
                .Include(quiz => quiz.Course)
                .Include(quiz => quiz.Questions.OrderBy(question => question.Order))
                .SingleOrDefaultAsync(quiz => quiz.Id == Id);
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] Could not get quiz {@Id} for taking, error message; {e}", Id, e.Message);
            throw;
        }
    }

    // Saves a quiz attempt to the db
    public async Task AddAttemptAsync(QuizAttempt attempt)
    {
        try
        {
            _db.QuizAttempts.Add(attempt);
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] Could not add the quiz attempt for quiz {@QuizId}, error message: {@e}", attempt.QuizId, e.Message);
            throw;
        }
    }

    // Pulls student attempt history by studentId and quizId
    public async Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int studentId)
    {
        try
        {
            return await _db.QuizAttempts
                .AsNoTracking()
                .Where(attempt => attempt.QuizId == quizId && attempt.StudentId == studentId)
                .Include(attempt => attempt.Result)
                .OrderBy(attempt => attempt.CompletedAt)
                .ThenBy(attempt => attempt.Id)
                .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] GetAttemptsForStudent for quiz {@quizId} failed, error message: {@e}", quizId, e.Message);
            throw;
        }
    }
}