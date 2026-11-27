using SubApp1.ViewModels;

namespace SubApp1.Services;

public interface IQuizService // Keeps quiz actions independent of data storage
{
    Task<TakeQuizViewModel?> GetQuizToTakeAsync(int quizId); // Returns null for an unknown quiz -> controller giver 404 error

    Task<QuizResultViewModel?> GradeQuizAsync(TakeQuizViewModel submission, int userId); // Returns null for invalid IDs
    Task<QuizResultViewModel?> GetLatestResultAsync(int quizId, int userId); // dashboard: null = no attempt yet
}