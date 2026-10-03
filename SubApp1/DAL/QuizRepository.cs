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

    public async Task<IEnumerable<Quiz>> GetAll()
    {
        return await _db.Quizzes.ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetAllCourses()
    {
        return await _db.Courses.ToListAsync();
    }

    public async Task<Quiz?> GetById(int id)
    {
        // a potential null is handled by controller, not repo
        return await _db.Quizzes.FindAsync(id);
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

    public async Task Create(Quiz quiz)
    {
        _db.Quizzes.Add(quiz);
        await _db.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var quiz = await _db.Quizzes.FindAsync(id);
        if (quiz != null)
        {
            _db.Quizzes.Remove(quiz);
            await _db.SaveChangesAsync();
        }
    }
}