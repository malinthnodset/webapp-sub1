using SubApp1.ViewModels;

namespace SubApp1.Services;

public class InMemoryQuizService : IQuizService // Temporary sample data and grading without a database
{
    private readonly object _historyLock = new();
    private readonly Dictionary<int, List<QuizAttemptHistoryViewModel>> _attemptHistory = new();

    private readonly IReadOnlyDictionary<int, QuizData> _quizzes = new Dictionary<int, QuizData> // Sample quiz and answer key
    {
        [1] = new QuizData(
            1,
            "Web basics",
            1,
            "Web Applications",
            new List<QuestionData>
            {
                new(1, "What does HTTP stand for?", "Hypertext Transfer Protocol", 1),
                new(2, "What is 2 + 2?", "4", 1, new[] { "3", "4", "5", "6" })
            })
    };

    public TakeQuizViewModel? GetQuizToTake(int quizId) // Returns form data without correct answers
    {
        if (!_quizzes.TryGetValue(quizId, out var quiz))
        {
            return null;
        }

        return new TakeQuizViewModel
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            Questions = quiz.Questions
                .Select(question => new TakeQuizQuestionViewModel // Keep the answer key server-side
                {
                    QuestionId = question.Id,
                    Prompt = question.Prompt,
                    Options = question.Options?.ToList() ?? new List<string>()
                })
                .ToList()
        };
    }

    public QuizResultViewModel? GradeQuiz(TakeQuizViewModel submission) // Validates IDs and calculates the score
    {
        // Form values are client-controlled, so require the exact set of question IDs for this quiz
        if (!_quizzes.TryGetValue(submission.QuizId, out var quiz) ||
            submission.Questions is null ||
            submission.Questions.Count != quiz.Questions.Count ||
            submission.Questions.Select(question => question.QuestionId).Distinct().Count() != quiz.Questions.Count) // Reject missing or duplicate question IDs
        {
            return null;
        }

        var submittedQuestions = submission.Questions.ToDictionary(question => question.QuestionId); // Match answers by question ID
        if (quiz.Questions.Any(question => !submittedQuestions.ContainsKey(question.Id)))
        {
            return null;
        }

        var answers = quiz.Questions.Select(question =>
        {
            // Ignore leading/trailing whitespace and letter case for this sample free-text quiz
            var submittedAnswer = submittedQuestions[question.Id].SubmittedAnswer?.Trim() ?? string.Empty; // Ignore surrounding spaces
            var isAvailableOption = question.Options is null ||
                question.Options.Count == 0 ||
                question.Options.Contains(submittedAnswer, StringComparer.OrdinalIgnoreCase);
            var isCorrect = string.Equals(
                submittedAnswer,
                question.CorrectAnswer,
                StringComparison.OrdinalIgnoreCase) && isAvailableOption; // Require a listed option for multiple-choice questions

            return new QuizAnswerResultViewModel
            {
                Prompt = question.Prompt,
                Options = question.Options?.ToList() ?? new List<string>(),
                SubmittedAnswer = submittedAnswer,
                CorrectAnswer = question.CorrectAnswer,
                IsCorrect = isCorrect,
                PointsAwarded = isCorrect ? question.Points : 0
            };
        }).ToList();

        // Calculate totals from the trusted question data, never from client-submitted point values
        var pointsEarned = answers.Sum(answer => answer.PointsAwarded); // Calculate totals from trusted quiz data
        var maximumPoints = quiz.Questions.Sum(question => question.Points);

        var result = new QuizResultViewModel
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            CourseId = quiz.CourseId,
            CourseTitle = quiz.CourseTitle,
            CompletedAt = DateTimeOffset.Now,
            Answers = answers,
            PointsEarned = pointsEarned,
            MaximumPoints = maximumPoints,
            CorrectAnswers = answers.Count(answer => answer.IsCorrect),
            TotalQuestions = answers.Count
        };

        lock (_historyLock)
        {
            if (!_attemptHistory.TryGetValue(quiz.Id, out var history))
            {
                history = new List<QuizAttemptHistoryViewModel>();
                _attemptHistory[quiz.Id] = history;
            }

            result.AttemptNumber = history.Count + 1;
            history.Add(new QuizAttemptHistoryViewModel
            {
                AttemptNumber = result.AttemptNumber,
                CompletedAt = result.CompletedAt,
                PointsEarned = result.PointsEarned,
                MaximumPoints = result.MaximumPoints
            });
            result.AttemptHistory = history.OrderByDescending(attempt => attempt.CompletedAt).ToList();
        }

        return result;
    }

    private sealed record QuizData(int Id, string Title, int CourseId, string CourseTitle, IReadOnlyList<QuestionData> Questions); // Quiz and course details

    private sealed record QuestionData(int Id, string Prompt, string CorrectAnswer, decimal Points, IReadOnlyList<string>? Options = null); // Answer key, points, and optional choices
}