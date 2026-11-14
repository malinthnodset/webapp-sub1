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

    // Searches for the quiz by it's Id, if it doesn't exist it returns null, if it does exist it maps 
    // the quiz and questions to a TakeQuizViewModel and returns it
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
            CourseCode = quiz.Course.Code,
            Questions = quiz.Questions
                .Select(question => new TakeQuizQuestionViewModel
                {
                    QuestionId = question.Id,
                    Prompt = question.Prompt
                })
                .ToList()
        };
    }

    // Grades the quiz by comparing the submitted answers to the correct answers, calculates the points 
    // earned, and saves the attempt and result to the database. then return the results of the quiz attempt 
    public async Task<QuizResultViewModel?> GradeQuizAsync(TakeQuizViewModel submission, int studentId)
    {
        //checks that the quiz exists, number of questions is correct and that question ids are in unique
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

        var startedAt = DateTime.UtcNow;    //// TODO: StartedAt is currently the same as CompletedAt, real start time not tracked yet
        var completedAt = DateTime.UtcNow;
        var answers = quiz.Questions.Select(question =>
        {
            var submittedAnswer = submittedQuestions[question.Id].SubmittedAnswer?.Trim() ?? string.Empty; //trims whitespace
            var isCorrect = string.Equals(
                submittedAnswer,
                question.CorrectAnswer,
                StringComparison.OrdinalIgnoreCase);    //ignores case when comparing answers

            return new QuizAnswerResultViewModel
            {
                Prompt = question.Prompt,
                SubmittedAnswer = submittedAnswer,
                CorrectAnswer = question.CorrectAnswer,
                IsCorrect = isCorrect,
                PointsAwarded = isCorrect ? question.Points : 0
            };
        }).ToList();

        //saves a quiz attempt and result in the database
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

        //builds the result page with the attempt history and the current attempt's results
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
            CourseCode = quiz.Course.Code,
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
// Dashboard: Rebuilds the result page for the student's latest attempt (same model as after grading)
public async Task<QuizResultViewModel?> GetLatestResultAsync(int quizId, int studentId)
{
    var attempt = await _quizRepository.GetLatestAttemptWithAnswersAsync(quizId, studentId);
    if (attempt is null || attempt.Result is null) return null;

    var attempts = await _quizRepository.GetAttemptsForStudentAsync(quizId, studentId);
    var ordered = attempts
        .Where(a => a.CompletedAt.HasValue && a.Result is not null)
        .OrderBy(a => a.CompletedAt).ThenBy(a => a.Id)
        .ToList();

    return new QuizResultViewModel
    {
        QuizId = attempt.QuizId,
        Title = attempt.Quiz.Title,
        CourseCode = attempt.Quiz.Course.Code,
        CourseTitle = attempt.Quiz.Course.Name,
        AttemptNumber = ordered.FindIndex(a => a.Id == attempt.Id) + 1,
        CompletedAt = ToUtcOffset(attempt.CompletedAt!.Value),
        AttemptHistory = ordered
            .Select((a, i) => new QuizAttemptHistoryViewModel
            {
                AttemptNumber = i + 1,
                CompletedAt = ToUtcOffset(a.CompletedAt!.Value),
                PointsEarned = a.Result!.PointsEarned,
                MaximumPoints = a.Result.MaximumPoints
            })
            .OrderByDescending(h => h.CompletedAt)
            .ToList(),
        Answers = attempt.Answers
            .OrderBy(a => a.QuizQuestionId)
            .Select(a => new QuizAnswerResultViewModel
            {
                Prompt = a.QuizQuestion.Prompt,
                SubmittedAnswer = a.SubmittedAnswer ?? string.Empty,
                CorrectAnswer = a.QuizQuestion.CorrectAnswer,
                IsCorrect = a.IsCorrect ?? false,
                PointsAwarded = a.PointsAwarded
            }).ToList(),
        PointsEarned = attempt.Result.PointsEarned,
        MaximumPoints = attempt.Result.MaximumPoints,
        CorrectAnswers = attempt.Result.CorrectAnswers,
        TotalQuestions = attempt.Result.TotalQuestions
    };
}

    private static DateTimeOffset ToUtcOffset(DateTime value)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }
}