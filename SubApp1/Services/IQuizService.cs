using SubApp1.ViewModels; // [LEARN] View models

namespace SubApp1.Services; // [LEARN] Services namespace

public interface IQuizService // [LEARN] Keeps quiz actions independent of data storage
{
    Task<TakeQuizViewModel?> GetQuizToTakeAsync(int quizId); // [LEARN] Returns null for an unknown quiz

    Task<QuizResultViewModel?> GradeQuizAsync(TakeQuizViewModel submission, int studentId); // [LEARN] Returns null for invalid IDs
}
