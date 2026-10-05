using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SubApp1.ViewModels;

// represents the content data the creation page needs

// Quiz details
public class QuizCreateViewModel
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Choose a course")]
    public int? CourseId { get; set; }

    [MaxLength(50, ErrorMessage = "A quiz can have at most 50 questions.")]
    public List<QuestionInput> Questions { get; set; } = [];

    // for the course dropdown menu
    public IEnumerable<SelectListItem>? Courses { get; set; }   
}

// Quiz questions - for text answer with 1 correct reply
public class QuestionInput
{
    [Required, StringLength(250)]
    public string Prompt { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string CorrectAnswer { get; set; } = string.Empty;

    [Range(0.01, 10)]
    public decimal Points { get; set; } = 1;
}

// TODO: add a multiple choice Q option