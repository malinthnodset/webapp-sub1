using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            // FindAsync would return an obj, we only need to know of existence -> AnySync
            // AnySync returns a bool-value, therefore no need for if/else returning either true or false
            return await _db.Courses.AnyAsync(course => course.Id == id); // returns true is any is found, otherwise false
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Failed to check whether CourseId {CourseId} exists, error message: {e}", id, e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Course>> GetAllCourses()
    {
        try
        {
            return await _db.Courses.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] courses ToListAsync() failed when GetAllCourses(), error message: {e}", e.Message);
            throw;
        }
    }

    public async Task<Course?> GetCourseById(int id)
    {
        try
        {
            return await _db.Courses.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError("[QuizRepository] GetCourseById for course {@id} failed, error message: {e}", id, e.Message);
            throw;
        }
    }

    public async Task<bool> Create(Course course)
    {
        try
        {
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] course creation failed for {@course}, error message: {e}", course, e.Message);
            throw;
        }
    }

    public async Task<bool> Update(Course course)
    {
        try
        {
            var existingCourse = await _db.Courses.FindAsync(course);
            if (existingCourse == null)
            {
                return false; // Course does not exist
            }
            _db.Courses.Update(existingCourse);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CourseRepository] Could not update course {@course}, error message {e}:", course, e.Message);
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var existingCourse = await _db.Courses.FindAsync(id);
            if (existingCourse is null)
            {
                return false;
            }

            _db.Courses.Remove(existingCourse);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete course {@id}, error message: {@e}", id, e.Message);
            throw;
        }
    }
}