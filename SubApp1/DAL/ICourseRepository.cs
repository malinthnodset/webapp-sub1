using SubApp1.Models;

namespace SubApp1.DAL;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllCourses(); // for selecting a course the quiz belongs to
    Task<Course?> GetCourseById(int id); // ? in case the course does not exist
    Task<bool> CourseExists(int id); // checks that course exists
    Task<bool> Create(Course course); // returns true when saved; database errors propagate
    Task<bool> Update(Course course); // edit course by id
    Task<bool> Delete(int id); // delete course by id
}