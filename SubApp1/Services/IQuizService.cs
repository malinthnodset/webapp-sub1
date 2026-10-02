using SubApp1.ViewModel;

namespace SubApp1.Services;

public interface IQuizService // Keeps quiz actions independent of data storage
{
    TakeQuizViewModel? GetQuizToTake(int quizId); // Returns null for an unknown quiz

    QuizResultViewModel? GradeQuiz(TakeQuizViewModel submission); // Returns null for invalid IDs
}