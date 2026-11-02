using Microsoft.EntityFrameworkCore; // [LEARN] EF Core
using SubApp1.Models; // [LEARN] Entities

namespace SubApp1.DAL; // [LEARN] Data access namespace

public class QuizRepository : IQuizRepository // [LEARN] Repository for quizzes
{
    private readonly QuizDbContext _db; // [LEARN] EF context
    private readonly ILogger<QuizRepository> _logger; // [LEARN] Logger

    public QuizRepository(QuizDbContext db, ILogger<QuizRepository> logger) // [LEARN] Constructor via DI
    {
        _db = db; // [LEARN] Store context
        _logger = logger; // [LEARN] Store logger
    }

    public async Task<IEnumerable<Quiz>?> GetAllQuizzes() // [LEARN] Get all quizzes
    {
        try // [LEARN] Try
        {
            return await _db.Quizzes
                .Include(q => q.Course)
                .Include(q => q.Questions)
                .ToListAsync(); // [LEARN] Query all quizzes

        } catch (Exception e) // [LEARN] Handle errors
        {
            // [LEARN] in case of error - display error mssg and return null
            _logger.LogError("[QuizRepository] quizzes ToListAsync() failed when GetAll(), error message: {e}", e.Message); // [LEARN] Log error
            return null; // [LEARN] Null on error
        }
    }

    public async Task<Quiz?> GetQuizById(int id) // [LEARN] Get quiz by id
    {
        try  // [LEARN] Try
        {
            return await _db.Quizzes.FindAsync(id); // [LEARN] Find by PK
        } 
        catch (Exception e) { // [LEARN] Handle errors
            // [LEARN] if we cannot find and return the quiz by id
            _logger.LogError("[QuizRepository] quiz FindAsync(id) failed when GetQuizById for QuizId {QuizId:0000}, error message:", e.Message); // [LEARN] Log error
            return null; // [LEARN] Null on error
        }
    }

    // [LEARN] Creates new quiz and saves it. Return true if successful, returns false if it fails
    public async Task<bool> Create(Quiz quiz) // [LEARN] Create quiz
    {
        try  // [LEARN] Try
        {
            _db.Quizzes.Add(quiz); // [LEARN] Add to context
            await _db.SaveChangesAsync(); // [LEARN] Persist
            return true; // [LEARN] Success
        } 
        catch (Exception e) { // [LEARN] Handle errors
            _logger.LogError("[QuizRepository] quiz creation failed for quiz {@quiz}, error message:", e.Message); // [LEARN] Log error
            return false; // [LEARN] Failure
        }
    }

    // [LEARN] marks a quiz as updated and saves it, same return true/false as create
    public async Task<bool> Update(Quiz quiz) // [LEARN] Update quiz
    {
        try // [LEARN] Try
        {
            _db.Quizzes.Update(quiz); // [LEARN] Mark updated
            await _db.SaveChangesAsync(); // [LEARN] Persist
            return true; // [LEARN] Success
        }
        catch (Exception e) // [LEARN] Handle errors
        {
            _logger.LogError("[QuizRepository] quiz FindAsync(id) failed when updateing the QuizId {QuizId:0000}, error message {e}:", quiz, e.Message); // [LEARN] Log error
            return false; // [LEARN] Failure
        }
    }

    // [LEARN] searches for quiz by it's ID, if it doesn't exist it returns false
    // [LEARN] if it exists, it is deleted and the changes saved
    public async Task<bool> Delete(int id) // [LEARN] Delete quiz
    {
        try // [LEARN] Try
        {
            var quiz = await _db.Quizzes.FindAsync(id); // [LEARN] Find quiz
            if (quiz is null) // [LEARN] Not found
            {
                return false; // [LEARN] Nothing to delete
            }

            _db.Quizzes.Remove(quiz); // [LEARN] Remove from context
            await _db.SaveChangesAsync(); // [LEARN] Persist
            return true; // [LEARN] Success
        }
        catch (Exception e) // [LEARN] Handle errors
        {
            _logger.LogError(e, "Failed to delete quiz {QuizId}", id); // [LEARN] Log error
            return false; // [LEARN] Failure
        }
    }

    // [LEARN] TAKE QUIZ METHODS
    public async Task<Quiz?> GetQuizForTakingAsync(int id) // [LEARN] Load quiz for taking
    {
        return await _db.Quizzes // [LEARN] Query quizzes
            .AsNoTracking() // [LEARN] Read-only, no change tracking
            .Include(quiz => quiz.Course) // [LEARN] Include course
            .Include(quiz => quiz.Questions.OrderBy(question => question.Order)) // [LEARN] Include ordered questions
            .SingleOrDefaultAsync(quiz => quiz.Id == id); // [LEARN] Single or null
    }

    // [LEARN] Saves a quiz attempt to the db
    public async Task AddAttemptAsync(QuizAttempt attempt) // [LEARN] Save attempt
    {
        _db.QuizAttempts.Add(attempt); // [LEARN] Add to context
        await _db.SaveChangesAsync(); // [LEARN] Persist
    }

    // [LEARN] Pulls student attempt history by studentId and quizId
    public async Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int studentId) // [LEARN] Get attempts
    {
        return await _db.QuizAttempts // [LEARN] Query attempts
            .AsNoTracking() // [LEARN] Read-only
            .Where(attempt => attempt.QuizId == quizId && attempt.StudentId == studentId) // [LEARN] Filter
            .Include(attempt => attempt.Result) // [LEARN] Include result
            .OrderBy(attempt => attempt.CompletedAt) // [LEARN] Order by completion
            .ThenBy(attempt => attempt.Id) // [LEARN] Then by id
            .ToListAsync(); // [LEARN] To list
    }
}
