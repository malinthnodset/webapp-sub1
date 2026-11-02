using SubApp1.Models; // [LEARN] Entities

namespace SubApp1.DAL; // [LEARN] Data access namespace

public interface ICourseRepository // [LEARN] Contract for course data access
{
    Task<IEnumerable<Course>?> GetAllCourses(); // [LEARN] for selecting a course the quiz belongs to
    Task<Quiz?> GetCourseById(int id); // [LEARN] ? in case the course does not exist
    Task<bool> CourseExists(int id); // [LEARN] checks that course exists
    Task<bool> Create(Course course); // [LEARN] setting datatype to bool = we get confirmation of success (true/false), so avoids silent failing
    Task<bool> Update (Course course); // [LEARN] edit course by id
    Task<bool> Delete(int id); // [LEARN] delete course by id
}
