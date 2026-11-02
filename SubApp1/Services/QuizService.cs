using SubApp1.DAL; // [LEARN] Data access
using SubApp1.Models; // [LEARN] Entities
using SubApp1.ViewModels; // [LEARN] View models

namespace SubApp1.Services; // [LEARN] Services namespace

public class QuizService : IQuizService // [LEARN] Quiz service
{
    private readonly IQuizRepository _quizRepository; // [LEARN] Repo field

    public QuizService(IQuizRepository quizRepository) // [LEARN] Constructor via DI
    {
        _quizRepository = quizRepository; // [LEARN] Store repo
    }

    // [LEARN] Searches for the quiz by it's Id, if it doesn't exist it returns null, if it does exist it maps 
    // [LEARN] the quiz and questions to a TakeQuizViewModel and returns it
    public async Task<TakeQuizViewModel?> GetQuizToTakeAsync(int quizId) // [LEARN] Load quiz to take
    {
        var quiz = await _quizRepository.GetQuizForTakingAsync(quizId); // [LEARN] Load from DB
        if (quiz is null) // [LEARN] Unknown quiz
        {
            return null; // [LEARN] Null
        }

        return new TakeQuizViewModel // [LEARN] Map to view model
        {
            QuizId = quiz.Id, // [LEARN] Id
            Title = quiz.Title, // [LEARN] Title
            Questions = quiz.Questions // [LEARN] Map questions
                .Select(question => new TakeQuizQuestionViewModel // [LEARN] Map
                {
                    QuestionId = question.Id, // [LEARN] Id
                    Prompt = question.Prompt // [LEARN] Prompt
                })
                .ToList() // [LEARN] To list
        };
    }

    // [LEARN] Grades the quiz by comparing the submitted answers to the correct answers, calculates the points 
    // [LEARN] earned, and saves the attempt and result to the database. then return the results of the quiz attempt 
    public async Task<QuizResultViewModel?> GradeQuizAsync(TakeQuizViewModel submission, int studentId) // [LEARN] Grade quiz
    {
        var quiz = await _quizRepository.GetQuizForTakingAsync(submission.QuizId); // [LEARN] Reload quiz
        if (quiz is null || // [LEARN] Unknown quiz
            submission.Questions is null || // [LEARN] No questions
            submission.Questions.Count != quiz.Questions.Count || // [LEARN] Count mismatch
            submission.Questions.Select(question => question.QuestionId).Distinct().Count() != quiz.Questions.Count) // [LEARN] Duplicate/missing ids
        {
            return null; // [LEARN] Invalid
        }

        var submittedQuestions = submission.Questions.ToDictionary(question => question.QuestionId); // [LEARN] Map by id
        if (quiz.Questions.Any(question => !submittedQuestions.ContainsKey(question.Id))) // [LEARN] Missing answers
        {
            return null; // [LEARN] Invalid
        }

        var startedAt = DateTime.UtcNow; // [LEARN] Start time
        var completedAt = DateTime.UtcNow; // [LEARN] Completion time
        var answers = quiz.Questions.Select(question => // [LEARN] Grade each question
        {
            var submittedAnswer = submittedQuestions[question.Id].SubmittedAnswer?.Trim() ?? string.Empty; // [LEARN] Trim answer
            var isCorrect = string.Equals( // [LEARN] Compare answers
                submittedAnswer,
                question.CorrectAnswer,
                StringComparison.OrdinalIgnoreCase);

            return new QuizAnswerResultViewModel // [LEARN] Build result
            {
                Prompt = question.Prompt, // [LEARN] Prompt
                SubmittedAnswer = submittedAnswer, // [LEARN] Submitted
                CorrectAnswer = question.CorrectAnswer, // [LEARN] Correct
                IsCorrect = isCorrect, // [LEARN] Correct?
                PointsAwarded = isCorrect ? question.Points : 0 // [LEARN] Points
            };
        }).ToList(); // [LEARN] To list

        var pointsEarned = answers.Sum(answer => answer.PointsAwarded); // [LEARN] Total points
        var maximumPoints = quiz.Questions.Sum(question => question.Points); // [LEARN] Max points
        var attempt = new QuizAttempt // [LEARN] Build attempt
        {
            QuizId = quiz.Id, // [LEARN] Quiz id
            StudentId = studentId, // [LEARN] Student id
            StartedAt = startedAt, // [LEARN] Start
            CompletedAt = completedAt // [LEARN] Complete
        };

        attempt.Answers = quiz.Questions.Select((question, index) => new QuizAnswer // [LEARN] Build answers
        {
            QuizAttempt = attempt, // [LEARN] Link attempt
            QuizQuestionId = question.Id, // [LEARN] Question id
            SubmittedAnswer = answers[index].SubmittedAnswer, // [LEARN] Submitted
            IsCorrect = answers[index].IsCorrect, // [LEARN] Correct?
            PointsAwarded = answers[index].PointsAwarded // [LEARN] Points
        }).ToList(); // [LEARN] To list

        attempt.Result = new QuizResult // [LEARN] Build result
        {
            QuizAttempt = attempt, // [LEARN] Link attempt
            PointsEarned = pointsEarned, // [LEARN] Earned
            MaximumPoints = maximumPoints, // [LEARN] Max
            CorrectAnswers = answers.Count(answer => answer.IsCorrect), // [LEARN] Correct count
            TotalQuestions = answers.Count // [LEARN] Total count
        };

        await _quizRepository.AddAttemptAsync(attempt); // [LEARN] Save attempt
        var attempts = await _quizRepository.GetAttemptsForStudentAsync(quiz.Id, studentId); // [LEARN] Load history
        var orderedAttempts = attempts // [LEARN] Filter completed
            .Where(savedAttempt => savedAttempt.CompletedAt.HasValue && savedAttempt.Result is not null)
            .OrderBy(savedAttempt => savedAttempt.CompletedAt) // [LEARN] Order
            .ThenBy(savedAttempt => savedAttempt.Id) // [LEARN] Then id
            .ToList(); // [LEARN] To list
        var currentAttemptNumber = orderedAttempts.FindIndex(savedAttempt => savedAttempt.Id == attempt.Id) + 1; // [LEARN] Attempt number

        return new QuizResultViewModel // [LEARN] Build view model
        {
            QuizId = quiz.Id, // [LEARN] Quiz id
            Title = quiz.Title, // [LEARN] Title
            CourseId = quiz.CourseId, // [LEARN] Course id
            CourseTitle = quiz.Course.Name, // [LEARN] Course name
            AttemptNumber = currentAttemptNumber, // [LEARN] Attempt number
            CompletedAt = ToUtcOffset(completedAt), // [LEARN] Completed at
            AttemptHistory = orderedAttempts // [LEARN] History
                .Select((savedAttempt, index) => new QuizAttemptHistoryViewModel // [LEARN] Map
                {
                    AttemptNumber = index + 1, // [LEARN] Number
                    CompletedAt = ToUtcOffset(savedAttempt.CompletedAt!.Value), // [LEARN] Completed
                    PointsEarned = savedAttempt.Result!.PointsEarned, // [LEARN] Earned
                    MaximumPoints = savedAttempt.Result.MaximumPoints // [LEARN] Max
                })
                .OrderByDescending(history => history.CompletedAt) // [LEARN] Order desc
                .ToList(), // [LEARN] To list
            Answers = answers, // [LEARN] Answers
            PointsEarned = pointsEarned, // [LEARN] Earned
            MaximumPoints = maximumPoints, // [LEARN] Max
            CorrectAnswers = answers.Count(answer => answer.IsCorrect), // [LEARN] Correct count
            TotalQuestions = answers.Count // [LEARN] Total count
        };
    }

    private static DateTimeOffset ToUtcOffset(DateTime value) // [LEARN] Helper
    {
        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)); // [LEARN] UTC offset
    }
}
