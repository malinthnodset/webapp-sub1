using System.ComponentModel.DataAnnotations; // [LEARN] Validation attributes

namespace SubApp1.ViewModels; // [LEARN] View models namespace

// [LEARN] Form data; correct answers stay out of the browser
public class TakeQuizViewModel // [LEARN] Take quiz view model
{
    public int QuizId { get; set; } // [LEARN] Quiz id

    public string Title { get; set; } = string.Empty; // [LEARN] Title

    public List<TakeQuizQuestionViewModel> Questions { get; set; } = new(); // [LEARN] Questions
}

// [LEARN] One prompt and it's submitted answer
public class TakeQuizQuestionViewModel // [LEARN] Take question
{
    public int QuestionId { get; set; } // [LEARN] Question id

    public string Prompt { get; set; } = string.Empty; // [LEARN] Prompt

    public List<string> Options { get; set; } = new(); // [LEARN] Options

    [Required(ErrorMessage = "Please enter an answer.")] // [LEARN] Validation
    [StringLength(2000)] // [LEARN] Validation
    public string? SubmittedAnswer { get; set; } // [LEARN] Submitted answer
}
