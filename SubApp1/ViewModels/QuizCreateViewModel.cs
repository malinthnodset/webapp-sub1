using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SubApp1.ViewModels;

// represents the data the creation page needs (so different from model-class)

// Quiz details
public class QuizCreateViewModel
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Choose a course")]
    public int? CourseId { get; set; }   // int? so "nothing chosen" is distinguishable from 0

    public List<QuestionInput> Questions { get; set; } = new();

    public IEnumerable<SelectListItem>? Courses { get; set; }   // for the course dropdown menu
}

// Quiz questions - for text answer with 1 correct reply
public class QuestionInput
{
    [Required, StringLength(200)]
    public string Prompt { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string CorrectAnswer { get; set; } = string.Empty;

    [Range(0.01, 1000)]
    public decimal Points { get; set; } = 1;
}

// TODO: add a multiple choice Q option