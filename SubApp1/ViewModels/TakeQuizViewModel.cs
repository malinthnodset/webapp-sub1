using System.ComponentModel.DataAnnotations;

namespace SubApp1.ViewModels;

// Form data; correct answers stay out of the browser
public class TakeQuizViewModel 
{
    public int QuizId { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<TakeQuizQuestionViewModel> Questions { get; set; } = new();
}

// One prompt and it's submitted answer
public class TakeQuizQuestionViewModel 
{
    public int QuestionId { get; set; }

    public string Prompt { get; set; } = string.Empty;

    public List<string> Options { get; set; } = new();

    [Required(ErrorMessage = "Please enter an answer.")]
    [StringLength(2000)]
    public string? SubmittedAnswer { get; set; }
}