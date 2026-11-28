using SubApp1.DAL;
using SubApp1.Models;
using SubApp1.Services;
using SubApp1.ViewModels;

namespace SubApp1.Tests;

// Unit tests for QuizService. 5 positive tests and 3 negative tests
public class QuizServiceTests
{
    private readonly FakeQuizRepository _repository = new();
    private readonly QuizService _service;

    public QuizServiceTests()
    {
        _service = new QuizService(_repository);
    }

    [Fact]
    public async Task GetQuizToTakeAsync_MapsQuizAndQuestions()
    {
        var quiz = await _service.GetQuizToTakeAsync(1);

        Assert.NotNull(quiz);
        Assert.Equal("Web basics", quiz.Title);
        Assert.Equal(2, quiz.Questions.Count);
        Assert.Empty(quiz.Questions[0].Options);
    }

    [Fact]
    public async Task GradeQuizAsync_WithAllCorrectAnswers_ReturnsFullScore()
    {
        var result = await _service.GradeQuizAsync(CreateSubmission(
            "Hypertext Transfer Protocol",
            "4"), userId: 1);

        Assert.NotNull(result);
        Assert.Equal(2m, result.PointsEarned);
        Assert.Equal(2m, result.MaximumPoints);
        Assert.Equal(2, result.CorrectAnswers);
        Assert.Equal(100m, result.Percentage);
        Assert.Equal("ITPE3200", result.CourseCode);
        Assert.Equal("Web Applications", result.CourseTitle);
        Assert.Empty(result.Answers[1].Options);
    }

    [Fact]
    public async Task GradeQuizAsync_IgnoresCaseAndSurroundingWhitespace()
    {
        var result = await _service.GradeQuizAsync(CreateSubmission(
            "  hypertext transfer protocol  ",
            " 4 "), userId: 1);

        Assert.NotNull(result);
        Assert.Equal(2m, result.PointsEarned);
    }

    [Fact]
    public async Task GradeQuizAsync_WithOneIncorrectAnswer_ReturnsPartialScore()
    {
        var result = await _service.GradeQuizAsync(CreateSubmission(
            "HTTP",
            "4"), userId: 1);

        Assert.NotNull(result);
        Assert.Equal(1m, result.PointsEarned);
        Assert.Equal(1, result.CorrectAnswers);
        Assert.False(result.Answers[0].IsCorrect);
        Assert.True(result.Answers[1].IsCorrect);
    }

    [Fact]
    public async Task GradeQuizAsync_WithUnknownQuiz_ReturnsNull()
    {
        var submission = CreateSubmission("answer", "answer");
        submission.QuizId = 999;

        var result = await _service.GradeQuizAsync(submission, userId: 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GradeQuizAsync_WithUnknownQuestionId_ReturnsNull()
    {
        var submission = CreateSubmission("answer", "answer");
        submission.Questions[0].QuestionId = 999;

        var result = await _service.GradeQuizAsync(submission, userId: 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GradeQuizAsync_WithMissingQuestion_ReturnsNull()
    {
        var submission = CreateSubmission("answer");

        var result = await _service.GradeQuizAsync(submission, userId: 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GradeQuizAsync_SavesAttemptsAndReturnsHistory()
    {
        var firstResult = await _service.GradeQuizAsync(CreateSubmission(
            "Hypertext Transfer Protocol",
            "4"), userId: 1);
        var secondResult = await _service.GradeQuizAsync(CreateSubmission(
            "HTTP",
            "4"), userId: 1);

        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Equal(1, firstResult.AttemptNumber);
        Assert.Single(firstResult.AttemptHistory);
        Assert.Equal(2, secondResult.AttemptNumber);
        Assert.Equal(new[] { 2, 1 }, secondResult.AttemptHistory.Select(attempt => attempt.AttemptNumber));
        Assert.NotEqual(default, secondResult.CompletedAt);
        Assert.Equal(2, _repository.SavedAttempts.Count);
    }

    private static TakeQuizViewModel CreateSubmission(params string[] answers)
    {
        return new TakeQuizViewModel
        {
            QuizId = 1,
            Questions = answers
                .Select((answer, index) => new TakeQuizQuestionViewModel
                {
                    QuestionId = index + 1,
                    SubmittedAnswer = answer
                })
                .ToList()
        };
    }

    private sealed class FakeQuizRepository : IQuizRepository
    {
        private readonly Quiz _quiz = new()
        {
            Id = 1,
            Title = "Web basics",
            CourseId = 1,
            Course = new Course { Id = 1, Code = "ITPE3200", Name = "Web Applications" },
            Questions = new List<QuizQuestion>
            {
                new() { Id = 1, QuizId = 1, Prompt = "What does HTTP stand for?", CorrectAnswer = "Hypertext Transfer Protocol", Points = 1, Order = 1 },
                new() { Id = 2, QuizId = 1, Prompt = "What is 2 + 2?", CorrectAnswer = "4", Points = 1, Order = 2 }
            }
        };

        public List<QuizAttempt> SavedAttempts { get; } = new();

        public Task<IEnumerable<Quiz>> GetAllQuizzes() => Task.FromResult<IEnumerable<Quiz>>(new[] { _quiz });

        public Task<Quiz?> GetQuizById(int id) => Task.FromResult<Quiz?>(id == _quiz.Id ? _quiz : null);

        public Task<Quiz?> GetQuizForTakingAsync(int id) => Task.FromResult<Quiz?>(id == _quiz.Id ? _quiz : null);

        public Task AddAttemptAsync(QuizAttempt attempt)
        {
            attempt.Id = SavedAttempts.Count + 1;
            SavedAttempts.Add(attempt);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int userId)
        {
            IReadOnlyList<QuizAttempt> attempts = SavedAttempts
                .Where(attempt => attempt.QuizId == quizId && attempt.UserId == userId)
                .ToList();
            return Task.FromResult(attempts);
        }

        public Task<bool> Create(Quiz quiz) => throw new NotSupportedException();

        public Task<bool> Update(Quiz quiz) => throw new NotSupportedException();

        public Task<bool> Delete(int id) => throw new NotSupportedException();

        public Task<QuizAttempt?> GetLatestAttemptWithAnswersAsync(int quizId, int userId)
        {
            throw new NotImplementedException();
        }
    }
}