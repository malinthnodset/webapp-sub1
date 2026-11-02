using System.ComponentModel.DataAnnotations; // [LEARN] Validation attributes
using Microsoft.AspNetCore.Mvc.Rendering; // [LEARN] SelectList

namespace SubApp1.ViewModels; // [LEARN] View models namespace

// [LEARN] represents the content data the creation page needs

// [LEARN] Quiz details
public class QuizCreateViewModel // [LEARN] Create quiz view model
{
    [Required, StringLength(200)] // [LEARN] Validation
    public string Title { get; set; } = string.Empty; // [LEARN] Title

    [StringLength(2000)] // [LEARN] Validation
    public string? Description { get; set; } // [LEARN] Description

    [Required(ErrorMessage = "Choose a course")] // [LEARN] Validation
    public int? CourseId { get; set; } // [LEARN] Course id

    [MaxLength(50, ErrorMessage = "A quiz can have at most 50 questions.")] // [LEARN] Validation
    public List<QuestionInput> Questions { get; set; } = []; // [LEARN] Questions list

    // [LEARN] for the course dropdown menu
    public IEnumerable<SelectListItem>? Courses { get; set; }   // [LEARN] Dropdown items
}

// [LEARN] Quiz questions - for text answer with 1 correct reply
public class QuestionInput // [LEARN] Question input
{
    [Required, StringLength(250)] // [LEARN] Validation
    public string Prompt { get; set; } = string.Empty; // [LEARN] Prompt

    [Required, StringLength(250)] // [LEARN] Validation
    public string CorrectAnswer { get; set; } = string.Empty; // [LEARN] Correct answer

    [Range(0.01, 10)] // [LEARN] Validation
    public decimal Points { get; set; } = 1; // [LEARN] Points
}

// [LEARN] TODO: add a multiple choice Q option
