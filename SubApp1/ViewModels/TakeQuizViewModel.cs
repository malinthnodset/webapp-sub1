using System.ComponentModel.DataAnnotations;

namespace SubApp1.ViewModels;

// Form data; correct answers stay out of the browser, without this the correct answers would be visible in the HTML
public class TakeQuizViewModel 
{
    public int QuizId { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<TakeQuizQuestionViewModel> Questions { get; set; } = new();
}

// One prompt and its submitted answer
public class TakeQuizQuestionViewModel 
{
    public int QuestionId { get; set; }

    public string Prompt { get; set; } = string.Empty;

    public List<string> Options { get; set; } = new();

// Server-side validation to ensure the user has entered a valid answer
    [Required(ErrorMessage = "Please enter an answer.")]
    [StringLength(2000)]
    public string? SubmittedAnswer { get; set; }
}