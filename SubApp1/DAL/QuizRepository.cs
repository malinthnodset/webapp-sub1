using Microsoft.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

public class QuizRepository : IQuizRepository
{
    // dependency injection - db
    private readonly QuizDbContext _db;

    public QuizRepository(QuizDbContext db)
    {
        _db = db;
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

    public async Task<Quiz?> GetQuizForTakingAsync(int id)
    {
        return await _db.Quizzes
            .AsNoTracking()
            .Include(quiz => quiz.Course)
            .Include(quiz => quiz.Questions.OrderBy(question => question.Order))
            .SingleOrDefaultAsync(quiz => quiz.Id == id);
    }

    public async Task AddAttemptAsync(QuizAttempt attempt)
    {
        _db.QuizAttempts.Add(attempt);
        await _db.SaveChangesAsync();
    }

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

    public async Task<Quiz?> GetQuizForTakingAsync(int id)
    {
        return await _db.Quizzes
            .AsNoTracking()
            .Include(quiz => quiz.Course)
            .Include(quiz => quiz.Questions.OrderBy(question => question.Order))
            .SingleOrDefaultAsync(quiz => quiz.Id == id);
    }

    public async Task AddAttemptAsync(QuizAttempt attempt)
    {
        _db.QuizAttempts.Add(attempt);
        await _db.SaveChangesAsync();
    }

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

    public async Task<bool> Delete(int id)
    {
        var quiz = await _db.Quizzes.FindAsync(id);
        if (quiz != null)
        {
            _db.Quizzes.Remove(quiz);
            await _db.SaveChangesAsync();
        }
    }
}