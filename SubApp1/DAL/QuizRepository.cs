using Microsoft.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

public class QuizRepository : IQuizRepository
{
    // dependency injection - db and logger
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
        try {
            var quiz = await _db.Quizzes.FindAsync(id);
            // check if quiz exists (id found)
            if (quiz == null)
            {
                _logger.LogError("[QuizRepository] quiz not found for the QuizId {QuizId:0000}", id);
                return false; 
            }

            _db.Quizzes.Remove(quiz);
            await _db.SaveChangesAsync();
            return true;
        } 
        catch (Exception e)
        {
             _logger.LogError("[QuizRepository] quiz deletion failed for QuizId {QuizId:0000}, error message: {e}", id, e.Message);
            return false;
        }
    }    
}