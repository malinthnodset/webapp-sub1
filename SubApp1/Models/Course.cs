using System.ComponentModel.DataAnnotations; // [LEARN] Validation attributes

namespace SubApp1.Models; // [LEARN] Domain entities namespace

public class Course // [LEARN] Course entity
{
    // [LEARN] Constructor - ensure a course cannot be created without a course code and name
    public Course(String code, String name) {
        this.Code = code; // [LEARN] Set code
        this.Name = name; // [LEARN] Set name
    }
    public int Id { get; set; } // [LEARN] Primary key

    [Required, StringLength(50)] // [LEARN] Validation
    public string Code { get; set; } = string.Empty; // [LEARN] Allows course code to be "" - handled with constructor

    [Required, StringLength(200)] // [LEARN] Validation
    public string Name { get; set; } = string.Empty; // [LEARN] Course name
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>(); // [LEARN] Navigation: quizzes in course

}
