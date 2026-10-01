using SubApp1.ViewModel;

namespace SubApp1.Services;

public class InMemoryQuizService : IQuizService
{
    private readonly IReadOnlyDictionary<int, QuizData> _quizzes = new Dictionary<int, QuizData>
    {
        [1] = new QuizData(
            1,
            "Web basics",
            new List<QuestionData>
            {
                new(1, "What does HTTP stand for?", "Hypertext Transfer Protocol", 1),
                new(2, "What is 2 + 2?", "4", 1)
            })
    };

    public TakeQuizViewModel? GetQuizToTake(int quizId)
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
                .Select(question => new TakeQuizQuestionViewModel
                {
                    QuestionId = question.Id,
                    Prompt = question.Prompt
                })
                .ToList()
        };
    }

    public QuizResultViewModel? GradeQuiz(TakeQuizViewModel submission)
    {
        if (!_quizzes.TryGetValue(submission.QuizId, out var quiz) ||
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

        return new QuizResultViewModel
        {
            Title = quiz.Title,
            Answers = answers,
            PointsEarned = pointsEarned,
            MaximumPoints = maximumPoints,
            CorrectAnswers = answers.Count(answer => answer.IsCorrect),
            TotalQuestions = answers.Count
        };
    }

    private sealed record QuizData(int Id, string Title, IReadOnlyList<QuestionData> Questions);

    private sealed record QuestionData(int Id, string Prompt, string CorrectAnswer, decimal Points);
}