using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

public class Course
{
    // constructor - ensure a course cannot be created without a course code and name
    public Course(String code, String name) {
        this.Code = code;
        this.Name = name;
    }
    public int Id { get; set; }

    [Required(ErrorMessage = "Course must have a course code")] 
    [StringLength(10)]
    public string Code { get; set; } = string.Empty; // allows course code to be "" - handled with constructor

    
    [Required(ErrorMessage = "Course must have a name")] 
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    // Connect quizzes to the course
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

}