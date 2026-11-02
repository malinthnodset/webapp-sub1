using Microsoft.EntityFrameworkCore; // [LEARN] EF Core
using SubApp1.Models; // [LEARN] Entities

namespace SubApp1.DAL; // [LEARN] Data access namespace

public class CourseRepository : ICourseRepository // [LEARN] Repository for courses
{
    private readonly QuizDbContext _db; // [LEARN] EF context
    private readonly ILogger<CourseRepository> _logger; // [LEARN] Logger

    public CourseRepository(QuizDbContext db, ILogger<CourseRepository> logger) // [LEARN] Constructor via DI
    {
        _db = db; // [LEARN] Store context
        _logger = logger; // [LEARN] Store logger
    }

    public async Task<bool> CourseExists(int id) // [LEARN] Check if course exists
    {
        try // [LEARN] Try
        {
            // [LEARN] FindAsync would return an obj, we only need to know of existence -> AnySync (true/false)
            return await _db.Courses.AnyAsync(c => c.Id == id); // [LEARN] True if exists
        } 
        catch (Exception e) // [LEARN] Handle errors
        {
            _logger.LogError("[CourseRepository] Failed to check whether CourseId {CourseId} exists, error message: {e}", id, e.Message); // [LEARN] Log error
            return false; // [LEARN] Assume not exists on error
        }
    }

        public async Task<IEnumerable<Course>?> GetAllCourses() // [LEARN] Get all courses
    {
        try  // [LEARN] Try
        {
            return await _db.Courses.ToListAsync(); // [LEARN] Query all courses
        } 
        catch (Exception e) // [LEARN] Handle errors
        {
            _logger.LogError("[QuizRepository] courses ToListAsync() failed when GetAllCourses(), error message: {e}", e.Message); // [LEARN] Log error
            return null; // [LEARN] Null on error
        }
    }

    // [LEARN] NOT YET IMPLEMENTED
    public Task<Quiz?> GetCourseById(int id) // [LEARN] Not implemented
    {
        throw new NotImplementedException(); // [LEARN] TODO
    }

    public Task<bool> Create(Course course) // [LEARN] Not implemented
    {
        throw new NotImplementedException(); // [LEARN] TODO
    }

    public Task<bool> Update(Course course) // [LEARN] Not implemented
    {
        throw new NotImplementedException(); // [LEARN] TODO
    }

    public Task<bool> Delete(int id) // [LEARN] Not implemented
    {
        throw new NotImplementedException(); // [LEARN] TODO
    }
}
