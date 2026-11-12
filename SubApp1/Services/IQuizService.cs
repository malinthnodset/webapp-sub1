using SubApp1.ViewModels;

namespace SubApp1.Services;

public interface IQuizService // Keeps quiz actions independent of data storage
{
    Task<TakeQuizViewModel?> GetQuizToTakeAsync(int quizId); // Returns null for an unknown quiz -> controller giver 404 error

    Task<QuizResultViewModel?> GradeQuizAsync(TakeQuizViewModel submission, int studentId); // Returns null for invalid IDs
    Task<QuizResultViewModel?> GetLatestResultAsync(int quizId, int studentId); // dashboard: null = no attempt yet
}