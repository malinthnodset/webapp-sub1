using SubApp1.ViewModel;

namespace SubApp1.Services;

public interface IQuizService
{
    TakeQuizViewModel? GetQuizToTake(int quizId);

    QuizResultViewModel? GradeQuiz(TakeQuizViewModel submission);
}