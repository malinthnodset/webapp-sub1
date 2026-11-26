namespace SubApp1.Services;

// The game rules
public static class UnlockRules
{
    // The cat leaves the box after the first completed quiz
    public static bool CatIsFree(int completedQuizzes) => completedQuizzes >= 1; 

    // Every further distinct quiz gives one piece of furniture/progress item
    public static int FurnitureEarned(int completedQuizzes) => Math.Max(completedQuizzes - 1, 0); 

    // True only if the student has earned more furniture than they own
    public static bool ShouldUnlockFurniture (int completedQuizzes, int ownedFurniture)
        => FurnitureEarned(completedQuizzes) > ownedFurniture;
}