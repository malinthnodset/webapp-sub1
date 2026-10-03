using SubApp1.Services;
using SubApp1.ViewModels;

namespace SubApp1.Tests;

public class InMemoryQuizServiceTests // Tests scoring and submission validation.
{
    private readonly InMemoryQuizService _service = new();

    [Fact]
    public void GradeQuiz_WithAllCorrectAnswers_ReturnsFullScore() // Correct answers earn full points.
    {
        var result = _service.GradeQuiz(CreateSubmission(
            "Hypertext Transfer Protocol",
            "4"));

        Assert.NotNull(result);
        Assert.Equal(2m, result.PointsEarned);
        Assert.Equal(2m, result.MaximumPoints);
        Assert.Equal(2, result.CorrectAnswers);
        Assert.Equal(100m, result.Percentage);
        Assert.Equal(1, result.CourseId);
        Assert.Equal("Web Applications", result.CourseTitle);
        Assert.Equal(new[] { "3", "4", "5", "6" }, result.Answers[1].Options);
    }

    [Fact]
    public void GradeQuiz_IgnoresCaseAndSurroundingWhitespace() // Formatting differences still match.
    {
        var result = _service.GradeQuiz(CreateSubmission(
            "  hypertext transfer protocol  ",
            " 4 "));

        Assert.NotNull(result);
        Assert.Equal(2m, result.PointsEarned);
    }

    [Fact]
    public void GradeQuiz_WithOneIncorrectAnswer_ReturnsPartialScore() // Only the correct answer earns points.
    {
        var result = _service.GradeQuiz(CreateSubmission(
            "HTTP",
            "4"));

        Assert.NotNull(result);
        Assert.Equal(1m, result.PointsEarned);
        Assert.Equal(1, result.CorrectAnswers);
        Assert.False(result.Answers[0].IsCorrect);
        Assert.True(result.Answers[1].IsCorrect);
    }

    [Fact]
    public void GradeQuiz_WithUnlistedChoice_MarksAnswerIncorrect() // Multiple-choice answers must be one of the listed options.
    {
        var result = _service.GradeQuiz(CreateSubmission(
            "Hypertext Transfer Protocol",
            "7"));

        Assert.NotNull(result);
        Assert.Equal(1m, result.PointsEarned);
        Assert.False(result.Answers[1].IsCorrect);
    }

    [Fact]
    public void GradeQuiz_RecordsAttemptHistory() // The in-memory history includes current and previous attempts.
    {
        var firstResult = _service.GradeQuiz(CreateSubmission(
            "Hypertext Transfer Protocol",
            "4"));
        var secondResult = _service.GradeQuiz(CreateSubmission(
            "HTTP",
            "4"));

        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Equal(1, firstResult.AttemptNumber);
        Assert.Single(firstResult.AttemptHistory);
        Assert.Equal(2, secondResult.AttemptNumber);
        Assert.Equal(new[] { 2, 1 }, secondResult.AttemptHistory.Select(attempt => attempt.AttemptNumber));
        Assert.NotEqual(default, secondResult.CompletedAt);
    }

    [Fact]
    public void GradeQuiz_WithUnknownQuiz_ReturnsNull() // Unknown quizzes are rejected.
    {
        var submission = CreateSubmission("answer", "answer");
        submission.QuizId = 999;

        var result = _service.GradeQuiz(submission);

        Assert.Null(result);
    }

    [Fact]
    public void GradeQuiz_WithUnknownQuestionId_ReturnsNull() // Foreign question IDs are rejected.
    {
        var submission = CreateSubmission("answer", "answer");
        submission.Questions[0].QuestionId = 999;

        var result = _service.GradeQuiz(submission);

        Assert.Null(result);
    }

    private static TakeQuizViewModel CreateSubmission(string firstAnswer, string secondAnswer) // Builds the sample quiz submission.
    {
        return new TakeQuizViewModel
        {
            QuizId = 1,
            Questions = new List<TakeQuizQuestionViewModel>
            {
                new() { QuestionId = 1, SubmittedAnswer = firstAnswer },
                new() { QuestionId = 2, SubmittedAnswer = secondAnswer }
            }
        };
    }
}