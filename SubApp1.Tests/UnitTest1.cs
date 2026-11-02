using SubApp1.DAL; // [LEARN] Data access
using SubApp1.Models; // [LEARN] Entities
using SubApp1.Services; // [LEARN] Services
using SubApp1.ViewModels; // [LEARN] View models
using Microsoft.Data.Sqlite; // [LEARN] SQLite provider
using Microsoft.EntityFrameworkCore; // [LEARN] EF Core
using Microsoft.Extensions.Logging.Abstractions; // [LEARN] Null logger

namespace SubApp1.Tests; // [LEARN] Test namespace

public class QuizServiceTests // [LEARN] Tests for QuizService
{
    private readonly FakeQuizRepository _repository = new(); // [LEARN] Fake repo
    private readonly QuizService _service; // [LEARN] Service under test

    public QuizServiceTests() // [LEARN] Constructor
    {
        _service = new QuizService(_repository); // [LEARN] Create service
    }

    [Fact] // [LEARN] xUnit fact
    public async Task GetQuizToTakeAsync_MapsQuizAndQuestions() // [LEARN] Test mapping
    {
        var quiz = await _service.GetQuizToTakeAsync(1); // [LEARN] Load quiz

        Assert.NotNull(quiz); // [LEARN] Not null
        Assert.Equal("Web basics", quiz.Title); // [LEARN] Title
        Assert.Equal(2, quiz.Questions.Count); // [LEARN] Count
        Assert.Empty(quiz.Questions[0].Options); // [LEARN] No options
    }

    [Fact] // [LEARN] xUnit fact
    public async Task GradeQuizAsync_WithAllCorrectAnswers_ReturnsFullScore() // [LEARN] Test full score
    {
        var result = await _service.GradeQuizAsync(CreateSubmission( // [LEARN] Grade
            "Hypertext Transfer Protocol",
            "4"), studentId: 1);

        Assert.NotNull(result); // [LEARN] Not null
        Assert.Equal(2m, result.PointsEarned); // [LEARN] Points
        Assert.Equal(2m, result.MaximumPoints); // [LEARN] Max
        Assert.Equal(2, result.CorrectAnswers); // [LEARN] Correct count
        Assert.Equal(100m, result.Percentage); // [LEARN] Percentage
        Assert.Equal(1, result.CourseId); // [LEARN] Course id
        Assert.Equal("Web Applications", result.CourseTitle); // [LEARN] Course title
        Assert.Empty(result.Answers[1].Options); // [LEARN] No options
    }

    [Fact] // [LEARN] xUnit fact
    public async Task GradeQuizAsync_IgnoresCaseAndSurroundingWhitespace() // [LEARN] Test case/whitespace
    {
        var result = await _service.GradeQuizAsync(CreateSubmission( // [LEARN] Grade
            "  hypertext transfer protocol  ",
            " 4 "), studentId: 1);

        Assert.NotNull(result); // [LEARN] Not null
        Assert.Equal(2m, result.PointsEarned); // [LEARN] Points
    }

    [Fact] // [LEARN] xUnit fact
    public async Task GradeQuizAsync_WithOneIncorrectAnswer_ReturnsPartialScore() // [LEARN] Test partial
    {
        var result = await _service.GradeQuizAsync(CreateSubmission( // [LEARN] Grade
            "HTTP",
            "4"), studentId: 1);

        Assert.NotNull(result); // [LEARN] Not null
        Assert.Equal(1m, result.PointsEarned); // [LEARN] Points
        Assert.Equal(1, result.CorrectAnswers); // [LEARN] Correct count
        Assert.False(result.Answers[0].IsCorrect); // [LEARN] First wrong
        Assert.True(result.Answers[1].IsCorrect); // [LEARN] Second right
    }

    [Fact] // [LEARN] xUnit fact
    public async Task GradeQuizAsync_WithUnknownQuiz_ReturnsNull() // [LEARN] Test unknown quiz
    {
        var submission = CreateSubmission("answer", "answer"); // [LEARN] Submission
        submission.QuizId = 999; // [LEARN] Unknown id

        var result = await _service.GradeQuizAsync(submission, studentId: 1); // [LEARN] Grade

        Assert.Null(result); // [LEARN] Null
    }

    [Fact] // [LEARN] xUnit fact
    public async Task GradeQuizAsync_WithUnknownQuestionId_ReturnsNull() // [LEARN] Test unknown question
    {
        var submission = CreateSubmission("answer", "answer"); // [LEARN] Submission
        submission.Questions[0].QuestionId = 999; // [LEARN] Unknown id

        var result = await _service.GradeQuizAsync(submission, studentId: 1); // [LEARN] Grade

        Assert.Null(result); // [LEARN] Null
    }

    [Fact] // [LEARN] xUnit fact
    public async Task GradeQuizAsync_WithMissingQuestion_ReturnsNull() // [LEARN] Test missing question
    {
        var submission = CreateSubmission("answer"); // [LEARN] One answer

        var result = await _service.GradeQuizAsync(submission, studentId: 1); // [LEARN] Grade

        Assert.Null(result); // [LEARN] Null
    }

    [Fact] // [LEARN] xUnit fact
    public async Task GradeQuizAsync_SavesAttemptsAndReturnsHistory() // [LEARN] Test history
    {
        var firstResult = await _service.GradeQuizAsync(CreateSubmission( // [LEARN] First attempt
            "Hypertext Transfer Protocol",
            "4"), studentId: 1);
        var secondResult = await _service.GradeQuizAsync(CreateSubmission( // [LEARN] Second attempt
            "HTTP",
            "4"), studentId: 1);

        Assert.NotNull(firstResult); // [LEARN] Not null
        Assert.NotNull(secondResult); // [LEARN] Not null
        Assert.Equal(1, firstResult.AttemptNumber); // [LEARN] Attempt number
        Assert.Single(firstResult.AttemptHistory); // [LEARN] History count
        Assert.Equal(2, secondResult.AttemptNumber); // [LEARN] Attempt number
        Assert.Equal(new[] { 2, 1 }, secondResult.AttemptHistory.Select(attempt => attempt.AttemptNumber)); // [LEARN] Order
        Assert.NotEqual(default, secondResult.CompletedAt); // [LEARN] Completed
        Assert.Equal(2, _repository.SavedAttempts.Count); // [LEARN] Saved count
    }

    [Fact] // [LEARN] xUnit fact
    public async Task SeededWebBasicsQuiz_HasTenQuestionsAndCanSaveAnAttempt() // [LEARN] Integration-style test
    {
        await using var connection = new SqliteConnection("Data Source=:memory:"); // [LEARN] In-memory SQLite
        await connection.OpenAsync(); // [LEARN] Open

        var options = new DbContextOptionsBuilder<QuizDbContext>() // [LEARN] Options
            .UseSqlite(connection)
            .Options;
        await using var db = new QuizDbContext(options); // [LEARN] Context
        await db.Database.EnsureCreatedAsync(); // [LEARN] Create schema

        // [LEARN] sees the database is empty, so we seed it with a quiz with 10 questions
        var course = new Course("ITPE3200", "Web applications"); // [LEARN] Course
        var student = new Student { Name = "Bob Johansen", Email = "bob@johansen.no" }; // [LEARN] Student

        db.Quizzes.Add(new Quiz // [LEARN] Quiz
        {
            Id = 1000,
            Title = "Web basics",
            Description = "Test quiz",
            Course = course,
            CreatedByStudent = student,
            Questions = Enumerable.Range(1, 10) // [LEARN] 10 questions
                .Select(i => new QuizQuestion
                {
                    Prompt = $"Question {i}",
                    CorrectAnswer = $"Answer {i}",
                    Points = 1,
                    Order = i
                })
                .ToList()
        });
        await db.SaveChangesAsync(); // [LEARN] Persist

        var repository = new QuizRepository(db, NullLogger<QuizRepository>.Instance); // [LEARN] Repo
        var service = new QuizService(repository); // [LEARN] Service
        var quiz = await service.GetQuizToTakeAsync(1000); // [LEARN] Load
        var questions = await db.QuizQuestions // [LEARN] Questions
            .Where(question => question.QuizId == 1000)
            .OrderBy(question => question.Order)
            .ToListAsync();

        Assert.NotNull(quiz); // [LEARN] Not null
        Assert.Equal("Web basics", quiz.Title); // [LEARN] Title
        Assert.Equal(10, quiz.Questions.Count); // [LEARN] Count

        var submission = new TakeQuizViewModel // [LEARN] Submission
        {
            QuizId = 1000,
            Questions = questions.Select(question => new TakeQuizQuestionViewModel
            {
                QuestionId = question.Id,
                SubmittedAnswer = question.CorrectAnswer
            }).ToList()
        };
        var result = await service.GradeQuizAsync(submission, studentId: 1); // [LEARN] Grade

        Assert.NotNull(result); // [LEARN] Not null
        Assert.Equal(10m, result.PointsEarned); // [LEARN] Points
        Assert.Equal(1, await db.QuizAttempts.CountAsync()); // [LEARN] Attempts
        Assert.Equal(10, await db.QuizAnswers.CountAsync()); // [LEARN] Answers
        Assert.Equal(1, await db.QuizResults.CountAsync()); // [LEARN] Results

        var persistedHistory = await repository.GetAttemptsForStudentAsync(result.QuizId, studentId: 1); // [LEARN] History
        Assert.Single(persistedHistory); // [LEARN] One
        Assert.NotNull(persistedHistory[0].Result); // [LEARN] Result
    }

    private static TakeQuizViewModel CreateSubmission(params string[] answers) // [LEARN] Helper
    {
        return new TakeQuizViewModel // [LEARN] Build submission
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

    private sealed class FakeQuizRepository : IQuizRepository // [LEARN] Fake repo
    {
        private readonly Quiz _quiz = new() // [LEARN] Seed quiz
        {
            Id = 1,
            Title = "Web basics",
            CourseId = 1,
            Course = new Course("ITPE3200", "Web Applications") { Id = 1 },
            Questions = new List<QuizQuestion>
            {
                new() { Id = 1, QuizId = 1, Prompt = "What does HTTP stand for?", CorrectAnswer = "Hypertext Transfer Protocol", Points = 1, Order = 1 },
                new() { Id = 2, QuizId = 1, Prompt = "What is 2 + 2?", CorrectAnswer = "4", Points = 1, Order = 2 }
            }
        };

        public List<QuizAttempt> SavedAttempts { get; } = new(); // [LEARN] Saved attempts

        public Task<IEnumerable<Quiz>?> GetAllQuizzes() => Task.FromResult<IEnumerable<Quiz>?>(new[] { _quiz }); // [LEARN]

        public Task<Quiz?> GetQuizById(int id) => Task.FromResult<Quiz?>(id == _quiz.Id ? _quiz : null); // [LEARN]

        public Task<Quiz?> GetQuizForTakingAsync(int id) => Task.FromResult<Quiz?>(id == _quiz.Id ? _quiz : null); // [LEARN]

        public Task AddAttemptAsync(QuizAttempt attempt) // [LEARN]
        {
            attempt.Id = SavedAttempts.Count + 1; // [LEARN] Set id
            SavedAttempts.Add(attempt); // [LEARN] Save
            return Task.CompletedTask; // [LEARN]
        }

        public Task<IReadOnlyList<QuizAttempt>> GetAttemptsForStudentAsync(int quizId, int studentId) // [LEARN]
        {
            IReadOnlyList<QuizAttempt> attempts = SavedAttempts // [LEARN]
                .Where(attempt => attempt.QuizId == quizId && attempt.StudentId == studentId)
                .ToList();
            return Task.FromResult(attempts); // [LEARN]
        }

        public Task<bool> Create(Quiz quiz) => throw new NotSupportedException(); // [LEARN]

        public Task<bool> Update(Quiz quiz) => throw new NotSupportedException(); // [LEARN]

        public Task<bool> Delete(int id) => throw new NotSupportedException(); // [LEARN]
    }
}
