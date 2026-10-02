using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

public class Course
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Code { get; set; } // ?

    // all quizes linked to a course
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}