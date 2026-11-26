using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

public class Course
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Course must have a course code")]
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course must have a name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    // Navgivation property - connect quizzes to the course
    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}