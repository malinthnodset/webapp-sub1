using SubApp1.DAL;
using SubApp1.Models;
using SubApp1.ViewModels;

namespace SubApp1.Services;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;

    public QuizService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<TakeQuizViewModel?> GetQuizToTakeAsync(int quizId)
    {
        var quiz = await _quizRepository.GetQuizForTakingAsync(quizId);
        if (quiz is null)
        {
            return null;
        }

        return new TakeQuizViewModel
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            Questions = quiz.Questions
                .Select(question => new TakeQuizQuestionViewModel
                {
                    QuestionId = question.Id,
                    Prompt = question.Prompt
                })
                .ToList()
        };
    }

    public async Task<QuizResultViewModel?> GradeQuizAsync(TakeQuizViewModel submission, int studentId)
    {
        var quiz = await _quizRepository.GetQuizForTakingAsync(submission.QuizId);
        if (quiz is null ||
            submission.Questions is null ||
            submission.Questions.Count != quiz.Questions.Count ||
            submission.Questions.Select(question => question.QuestionId).Distinct().Count() != quiz.Questions.Count)
        {
            return null;
        }

        var submittedQuestions = submission.Questions.ToDictionary(question => question.QuestionId);
        if (quiz.Questions.Any(question => !submittedQuestions.ContainsKey(question.Id)))
        {
            return null;
        }

        var startedAt = DateTime.UtcNow;
        var completedAt = DateTime.UtcNow;
        var answers = quiz.Questions.Select(question =>
        {
            var submittedAnswer = submittedQuestions[question.Id].SubmittedAnswer?.Trim() ?? string.Empty;
            var isCorrect = string.Equals(
                submittedAnswer,
                question.CorrectAnswer,
                StringComparison.OrdinalIgnoreCase);

            return new QuizAnswerResultViewModel
            {
                Prompt = question.Prompt,
                SubmittedAnswer = submittedAnswer,
                CorrectAnswer = question.CorrectAnswer,
                IsCorrect = isCorrect,
                PointsAwarded = isCorrect ? question.Points : 0
            };
        }).ToList();

        var pointsEarned = answers.Sum(answer => answer.PointsAwarded);
        var maximumPoints = quiz.Questions.Sum(question => question.Points);
        var attempt = new QuizAttempt
        {
            QuizId = quiz.Id,
            StudentId = studentId,
            StartedAt = startedAt,
            CompletedAt = completedAt
        };

        attempt.Answers = quiz.Questions.Select((question, index) => new QuizAnswer
        {
            QuizAttempt = attempt,
            QuizQuestionId = question.Id,
            SubmittedAnswer = answers[index].SubmittedAnswer,
            IsCorrect = answers[index].IsCorrect,
            PointsAwarded = answers[index].PointsAwarded
        }).ToList();

        attempt.Result = new QuizResult
        {
            QuizAttempt = attempt,
            PointsEarned = pointsEarned,
            MaximumPoints = maximumPoints,
            CorrectAnswers = answers.Count(answer => answer.IsCorrect),
            TotalQuestions = answers.Count
        };

        await _quizRepository.AddAttemptAsync(attempt);
        var attempts = await _quizRepository.GetAttemptsForStudentAsync(quiz.Id, studentId);
        var orderedAttempts = attempts
            .Where(savedAttempt => savedAttempt.CompletedAt.HasValue && savedAttempt.Result is not null)
            .OrderBy(savedAttempt => savedAttempt.CompletedAt)
            .ThenBy(savedAttempt => savedAttempt.Id)
            .ToList();
        var currentAttemptNumber = orderedAttempts.FindIndex(savedAttempt => savedAttempt.Id == attempt.Id) + 1;

        return new QuizResultViewModel
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            CourseId = quiz.CourseId,
            CourseTitle = quiz.Course.Name,
            AttemptNumber = currentAttemptNumber,
            CompletedAt = ToUtcOffset(completedAt),
            AttemptHistory = orderedAttempts
                .Select((savedAttempt, index) => new QuizAttemptHistoryViewModel
                {
                    AttemptNumber = index + 1,
                    CompletedAt = ToUtcOffset(savedAttempt.CompletedAt!.Value),
                    PointsEarned = savedAttempt.Result!.PointsEarned,
                    MaximumPoints = savedAttempt.Result.MaximumPoints
                })
                .OrderByDescending(history => history.CompletedAt)
                .ToList(),
            Answers = answers,
            PointsEarned = pointsEarned,
            MaximumPoints = maximumPoints,
            CorrectAnswers = answers.Count(answer => answer.IsCorrect),
            TotalQuestions = answers.Count
        };
    }

    private static DateTimeOffset ToUtcOffset(DateTime value)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }
}