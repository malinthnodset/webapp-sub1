using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SubApp1.ViewModels;

// represents the content data the creation page needs

// Quiz details
public class CreateQuizViewModel
{
    [Required(ErrorMessage = "The quiz must have a name.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Choose a course.")]
    public int? CourseId { get; set; }

    // for the course dropdown menu
    public IEnumerable<SelectListItem>? Courses { get; set; }

    [MinLength(1, ErrorMessage = "A quiz must have at least one question.")]
    [MaxLength(50, ErrorMessage = "A quiz can have at most 50 Question.")]
    public List<Question> Question { get; set; } = [];
}

// Quiz Question - for text answer Question with 1 correct answer
public class Question
{
    [Required(ErrorMessage = "Please add a question.")]
    [StringLength(250)]
    public string Prompt { get; set; } = string.Empty;

    [Required(ErrorMessage = "Write the correct answer.")]
    [StringLength(250)]
    public string CorrectAnswer { get; set; } = string.Empty;

    [Range(0, 10)]
    public decimal Points { get; set; } = 1;
}

// TODO: add a multiple choice Q option