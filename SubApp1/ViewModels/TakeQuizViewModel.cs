using System.ComponentModel.DataAnnotations;

namespace SubApp1.ViewModels;

public class TakeQuizViewModel // Form data; correct answers stay out of the browser
{
    public int QuizId { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<TakeQuizQuestionViewModel> Questions { get; set; } = new();
}

public class TakeQuizQuestionViewModel // One prompt and its submitted answer
{
    public int QuestionId { get; set; }

    public string Prompt { get; set; } = string.Empty;

    public List<string> Options { get; set; } = new();

    [Required(ErrorMessage = "Please enter an answer.")]
    [StringLength(2000)]
    public string? SubmittedAnswer { get; set; }
}