using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

public class Course
{
    // constructor - ensure a course cannot be created without a course code ?
    public Course(String code, String name) {
        this.Code = code;
        this.Name = name;
    }
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Code { get; set; } = string.Empty; // allows course code to be "" - handle with constructor or input validation ?

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

}