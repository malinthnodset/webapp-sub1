using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SubApp1.Models;

namespace SubApp1.DAL;

public class QuizRepository : IQuizRepository
{
    // dependency injection - db
    private readonly QuizDbContext _db;
    private readonly ILogger<QuizRepository> _logger;

    public QuizRepository(QuizDbContext db, ILogger<QuizRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Quiz>?> GetAllQuizzes() // Quizzes
    {
        try
        {
            return await _db.Quizzes.ToListAsync();

        } catch (Exception e)
        {
            // in case of error - display error mssg and return null
            _logger.LogError("[QuizRepository] quizzes ToListAsync() failed when GetAll(), error message: {e}", e.Message);
            return null;
        }
    }

    public async Task<Quiz?> GetQuizById(int id)
    {
        try 
        {
            return await _db.Quizzes.FindAsync(id);
        } 
        catch (Exception e) {
            // if we cannot find and return the quiz by id
            _logger.LogError("[QuizRepository] quiz FindAsync(id) failed when GetQuizById for QuizId {QuizId:0000}, error message:", e.Message);
            return null;
        }
    }


// creates new quiz and saves it, return true if it is successful
// and returns false if it fails
    public async Task<bool> Create(Quiz quiz)
    {
        try 
        {
            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();
            return true;
        } 
        catch (Exception e) {
            _logger.LogError("[QuizRepository] quiz creation failed for quiz {@quiz}, error message:", e.Message);
            return false;
        }
    }

// marks a quiz as updates and saves it, sme return true/false as create
    public async Task<bool> Update(Quiz quiz)
    {
        try
        {
            _db.Quizzes.Update(quiz);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] quiz FindAsync(id) failed when updateing the QuizId {QuizId:0000}, error message {e}:", quiz, e.Message);
            return false; 
        }
    }

// searches for quiz by it's ID, if it doesn't exist it returns false
// if it exists, it is deleted and the changes saved
    public async Task<bool> Delete(int id)
    {
        try
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz is null)
            {
                return false;
            }

            _db.Quizzes.Remove(quiz);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete quiz {QuizId}", id);
            return false;
        }
    }

    // TAKE QUIZ METHODS
    public async Task<Quiz?> GetQuizForTakingAsync(int id)
    {
        return await _db.Quizzes
            .AsNoTracking()
            .Include(quiz => quiz.Course)
            .Include(quiz => quiz.Questions.OrderBy(question => question.Order))
            .SingleOrDefaultAsync(quiz => quiz.Id == id);
    }

// method that saves a quiz attempt to the db
    public async Task AddAttemptAsync(QuizAttempt attempt)
    {
        _db.QuizAttempts.Add(attempt);
        await _db.SaveChangesAsync();
    }

// pulls student attempt history by studentId and quizId
    public async Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int studentId)
    {
        return await _db.QuizAttempts
            .AsNoTracking()
            .Where(attempt => attempt.QuizId == quizId && attempt.StudentId == studentId)
            .Include(attempt => attempt.Result)
            .OrderBy(attempt => attempt.CompletedAt)
            .ThenBy(attempt => attempt.Id)
            .ToListAsync();
    }
}