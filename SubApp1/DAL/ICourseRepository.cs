using SubApp1.Models;

namespace SubApp1.DAL;

public interface ICourseRepository
{
    Task<IEnumerable<Course>?> GetAllCourses(); // for selecting a course the quiz belongs to
    Task<Quiz?> GetCourseById(int id); // ? in case the course does not exist
    Task<bool> CourseExists(int id); // checks that course exists
    Task<bool> Create(Course course); // setting datatype to bool = we get confirmation of success (true/false), so avoids silent failing
    Task<bool> Update (Course course); // edit course by id
    Task<bool> Delete(int id); // delete course by id
}