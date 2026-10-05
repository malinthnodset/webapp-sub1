using Microsoft.EntityFrameworkCore;
using SubApp1.Models;

namespace SubApp1.DAL;

public class CourseRepository : ICourseRepository
{
    private readonly QuizDbContext _db;
    private readonly ILogger<CourseRepository> _logger;

    public CourseRepository(QuizDbContext db, ILogger<CourseRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> CourseExists(int id)
    {
        try
        {
            // FindAsync would return an obj, we only need to know of existence -> AnySync (true/false)
            return await _db.Courses.AnyAsync(c => c.Id == id);
        } 
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Failed to check whether CourseId {CourseId} exists, error message: {e}", id, e.Message);
            return false;
        }
    }

        public async Task<IEnumerable<Course>?> GetAllCourses()
    {
        try 
        {
            return await _db.Courses.ToListAsync();
        } 
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] courses ToListAsync() failed when GetAllCourses(), error message: {e}", e.Message);
            return null;
        }
    }

    // NOT YET IMPLEMENTED
    public Task<Quiz?> GetCourseById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Create(Course course)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(Course course)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int id)
    {
        throw new NotImplementedException();
    }
}